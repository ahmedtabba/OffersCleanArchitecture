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
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostForAdminQuery;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsForAdminQueryWithPagination;
public class GetPostsForAdminWithPaginationQuery : IRequest<PaginatedList<GetPostsForAdminWithPaginationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public Guid GroceryId { get; set; }
}

public class GetPostsForAdminQueryWithPaginationHandler : IRequestHandler<GetPostsForAdminWithPaginationQuery, PaginatedList<GetPostsForAdminWithPaginationDto>>
{
    private readonly ILogger<GetPostsForAdminQueryWithPaginationHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly IUserContext _userContext;
    private readonly ICountryRepository _countryRepository;

    public GetPostsForAdminQueryWithPaginationHandler(ILogger<GetPostsForAdminQueryWithPaginationHandler> logger,
                                                      IMapper mapper,
                                                      IPostRepository postRepository,
                                                      IPostLocalizationRepository postLocalizationRepository,
                                                      IUserContext userContext,
                                                      ICountryRepository countryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _postRepository = postRepository;
        _postLocalizationRepository = postLocalizationRepository;
        _userContext = userContext;
        _countryRepository = countryRepository;
    }

    public async Task<PaginatedList<GetPostsForAdminWithPaginationDto>> Handle(GetPostsForAdminWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // admin will be authorized so we get country and language from token

        var userLanguageId = _userContext.GetLanguageIdOfUser();
        var userCountryId = _userContext.GetCountryIdOfUser();

        var posts = _postRepository.GetAllWithGroceries()
            .Where(p => p.Grocery.CountryId == userCountryId);

        posts = posts.Where(p => p.GroceryId == request.GroceryId);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            posts = posts.Where(p => p.Title.ToLower().Contains(request.SearchText.ToLower()));
        }

        var result = await posts
            .OrderBy(p => p.Title)
            .ProjectTo<GetPostsForAdminWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber,request.PageSize);
        // fill all localization of post and IsLiven
        await LocalizationHelper.FillPostLocalizationsAndIsLiven(result, _postLocalizationRepository, _mapper, _postRepository);
        // change DateTime of returned result of post dto according to user TimeZone
        foreach (var postDto in result.Items)
        {
            await PostHelper.ConvertDateTimeToTimeZone(postDto, _userContext, _countryRepository);
        }

        return result;
    }
}
