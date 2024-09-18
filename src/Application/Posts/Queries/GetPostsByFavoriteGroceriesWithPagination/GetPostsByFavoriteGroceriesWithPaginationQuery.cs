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
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByFavoriteGroceriesWithPagination;
public class GetPostsByFavoriteGroceriesWithPaginationQuery : IRequest<PaginatedList<GetPostsByFavoriteGroceriesWithPaginationDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchText { get; set; }
    public PostFilter PostFilter { get; set; }

}

public class GetPostsByFavoriteGroceriesWithPaginationQueryHandler : IRequestHandler<GetPostsByFavoriteGroceriesWithPaginationQuery, PaginatedList<GetPostsByFavoriteGroceriesWithPaginationDto>>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IUser _user;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly ILogger<GetPostsByFavoriteGroceriesWithPaginationQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly ICountryRepository _countryRepository;

    public GetPostsByFavoriteGroceriesWithPaginationQueryHandler(IMapper mapper,
                                                       IPostRepository postRepository,
                                                       IUser user,
                                                       IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                                       ILogger<GetPostsByFavoriteGroceriesWithPaginationQueryHandler> logger,
                                                       IUserContext userContext,
                                                       IPostLocalizationRepository postLocalizationRepository,
                                                       ICountryRepository countryRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _user = user;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _logger = logger;
        _userContext = userContext;
        _postLocalizationRepository = postLocalizationRepository;
        _countryRepository = countryRepository;
    }
    public async Task<PaginatedList<GetPostsByFavoriteGroceriesWithPaginationDto>> Handle(GetPostsByFavoriteGroceriesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        
        // get favorite groceries Ids for current user
        var userFavorateGrocery =  _favoraiteGroceryRepository.GetFavoraiteGroceriesWithGroceriesBy_UserId(_user.Id!)!.Select(x => x.GroceryId).ToList();
        
        // get posts of favorite groceries (userFavorateGrocery Ids)
        var posts = _postRepository.GetAll()
            .Where(p => userFavorateGrocery.Contains(p.GroceryId));
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
            .ProjectTo<GetPostsByFavoriteGroceriesWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        var userLanguageId = _userContext.GetLanguageIdOfUser();
        if (userLanguageId != Guid.Empty)
            await LocalizationHelper.FillPostLocalizationsAndIsLiven(result, userLanguageId, _postLocalizationRepository,_postRepository);
        // change DateTime of returned result of post dto according to user TimeZone
        foreach (var postDto in result.Items)
        {
            await PostHelper.ConvertDateTimeToTimeZone(postDto, _userContext, _countryRepository);
        }

        return result;
    }
}
