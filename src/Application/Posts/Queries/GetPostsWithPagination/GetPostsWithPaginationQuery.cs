using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsWithPagination;
public class GetPostsWithPaginationQuery : IRequest<PaginatedList<GetPostsWithPaginationDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchText { get; set; }
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public PostFilter PostFilter { get; set; }
}

public class GetPostsWithPaginationQueryHandler : IRequestHandler<GetPostsWithPaginationQuery, PaginatedList<GetPostsWithPaginationDto>>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetPostsWithPaginationQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IGroceryRepository _groceryRepository;

    public GetPostsWithPaginationQueryHandler(IMapper mapper,
                                              IPostRepository postRepository,
                                              ILogger<GetPostsWithPaginationQueryHandler> logger,
                                              IUserContext userContext,
                                              IPostLocalizationRepository postLocalizationRepository,
                                              ICountryRepository countryRepository,
                                              IGroceryRepository groceryRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _logger = logger;
        _userContext = userContext;
        _postLocalizationRepository = postLocalizationRepository;
        _countryRepository = countryRepository;
        _groceryRepository = groceryRepository;
    }
    public async Task<PaginatedList<GetPostsWithPaginationDto>> Handle(GetPostsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        
        // we return only posts in the same country of user
        var posts = _postRepository.GetAllWithGroceries()
            .Where(p => p.Grocery.CountryId == request.CountryId);


        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            posts = posts.Where(p => p.Title.ToLower().Contains(request.SearchText.ToLower()));
            //posts = posts.Where(t => t.Outer.Title.Contains(request.SearchText));
        }
        // you find definition of active post in PostHelper class
      
        switch (request.PostFilter)
        {
            case PostFilter.Active:
                posts = posts
                            .Where(p =>
                                   (p.IsActive) &&
                                   (p.StartDate != null) &&
                                   ((p.EndDate == null && p.StartDate < DateTime.UtcNow) ||
                                   (p.EndDate != null && (DateTime.UtcNow >= p.StartDate && DateTime.UtcNow <= p.EndDate)))
                                   );
                break;
            case PostFilter.Popular:
                posts = posts;
                break;
            case PostFilter.Inactive:
                posts = posts
                            .Where(p =>
                                   (!p.IsActive) ||
                                   (p.IsActive && p.StartDate == null) ||
                                   (p.IsActive && p.StartDate != null && p.EndDate == null && p.StartDate > DateTime.UtcNow) ||
                                   (p.IsActive && p.StartDate != null && p.EndDate != null && (DateTime.UtcNow <= p.StartDate || DateTime.UtcNow >= p.EndDate))
                                   );
                break;
            default:// default case 'All'
                posts = posts;
                break;
        }

        var result = await posts
            .OrderBy(p => p.Title)
            //.OrderBy(t => t.Outer.Title)
            .ProjectTo<GetPostsWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // fill localization according to user language and fill IsLiven
        // if we have user language we use FillPostLocalizationsAndIsLiven to fill data
        // but if we don't have user language we fill IsLiven property using PostHelper.IsPostActive
        if (request.LanguageId != Guid.Empty)
            await LocalizationHelper.FillPostLocalizationsAndIsLiven(result, request.LanguageId, _postLocalizationRepository, _postRepository);
        else
            foreach (var postDto in result.Items)
            {
                postDto.IsLiven = await PostHelper.IsPostActive(postDto.Id,_postRepository);
            }

        // change DateTime of returned result of post dto post dto according to user TimeZone
        foreach (var postDto in result.Items)
        {
            await PostHelper.ConvertDateTimeToTimeZone(postDto,_countryRepository,request.CountryId);
        }

        return result;
    }

    /*
    private class TransparentIdentifier<TOuter, TInner>
    {
        public TOuter Outer { get; set; }
        public TInner Inner { get; set; }

        public TransparentIdentifier(TOuter outer, TInner inner)
        {
            Outer = outer;
            Inner = inner;
        }
    }

    public class Mapping : Profile
    {
         public Mapping()
        {
            CreateMap<TransparentIdentifier<Post, Grocery>, GetPostsWithPaginationDto>();
        }
    }
    */

}
