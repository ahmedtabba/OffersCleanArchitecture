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
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByFavoriteGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
public class GetPostsByGroceryWithPaginationQuery : IRequest<PaginatedList<GetPostsByGroceryWithPaginationDto>>
{
    public Guid GroceryId { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public PostFilter PostFilter { get; set; }

}

public class GetPostsByGroceryWithPaginationQueryHandler : IRequestHandler<GetPostsByGroceryWithPaginationQuery, PaginatedList<GetPostsByGroceryWithPaginationDto>>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetPostsByGroceryWithPaginationQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly ICountryRepository _countryRepository;

    public GetPostsByGroceryWithPaginationQueryHandler(IMapper mapper,
                                                       IPostRepository postRepository,
                                                       ILogger<GetPostsByGroceryWithPaginationQueryHandler> logger,
                                                       IUserContext userContext,
                                                       IPostLocalizationRepository postLocalizationRepository,
                                                       ICountryRepository countryRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _logger = logger;
        _userContext = userContext;
        _postLocalizationRepository = postLocalizationRepository;
        _countryRepository = countryRepository;
    }
    public async Task<PaginatedList<GetPostsByGroceryWithPaginationDto>> Handle(GetPostsByGroceryWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var posts = _postRepository.GetAllWithGroceries()
            .Where(p => p.Grocery.CountryId == request.CountryId);

        posts = posts.Where(p => p.GroceryId == request.GroceryId);


        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            posts = posts.Where(p => p.Title.ToLower().Contains(request.SearchText.ToLower()));
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
            .ProjectTo<GetPostsByGroceryWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // fill localization according to user language and fill IsLiven
        // if we have user language we use FillPostLocalizationsAndIsLiven to fill data
        // but if we don't have user language we fill IsLiven property using PostHelper.IsPostActive
        if (request.LanguageId != Guid.Empty)
            await LocalizationHelper.FillPostLocalizationsAndIsLiven(result, request.LanguageId, _postLocalizationRepository,_postRepository);
        else
            foreach (var postDto in result.Items)
            {
                postDto.IsLiven = await PostHelper.IsPostActive(postDto.Id, _postRepository);
            }
        // change DateTime of returned result of post dto according to user TimeZone
        foreach (var postDto in result.Items)
        {
            await PostHelper.ConvertDateTimeToTimeZone(postDto, /*_userContext*/ _countryRepository,request.CountryId);
        }

        return result;
    }
}
