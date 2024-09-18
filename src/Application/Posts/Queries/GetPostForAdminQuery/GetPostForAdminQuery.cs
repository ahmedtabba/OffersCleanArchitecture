using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostForAdminQuery;
public class GetPostForAdminQuery : IRequest<GetPostForAdminDto>
{
    public Guid PostId { get; set; }
}

public class GetPostForAdminQueryHandler : IRequestHandler<GetPostForAdminQuery, GetPostForAdminDto>
{
    private readonly ILogger<GetPostForAdminQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IUserContext _userContext;
    private readonly ICountryRepository _countryRepository;

    public GetPostForAdminQueryHandler(ILogger<GetPostForAdminQueryHandler> logger,
                                       IMapper mapper,
                                       IPostRepository postRepository,
                                       IPostLocalizationRepository postLocalizationRepository,
                                       IGroceryRepository groceryRepository,
                                       IUserContext userContext,
                                       ICountryRepository countryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _postRepository = postRepository;
        _postLocalizationRepository = postLocalizationRepository;
        _groceryRepository = groceryRepository;
        _userContext = userContext;
        _countryRepository = countryRepository;
    }

    public async Task<GetPostForAdminDto> Handle(GetPostForAdminQuery request, CancellationToken cancellationToken)
    {
        // get post from DB
        var post = await _postRepository.GetByIdAsync(request.PostId);
        // mapping , but without localization or GroceryName or IsLiven
        var postWithFullLocalizationDto = _mapper.Map<GetPostForAdminDto>(post);
        //fill localization and GroceryName
        await LocalizationHelper.FillPostLocalizations(postWithFullLocalizationDto, _postLocalizationRepository,_mapper);
        postWithFullLocalizationDto.GroceryName = await _groceryRepository.GetGroceryNameByGroceryIdAsync(postWithFullLocalizationDto.GroceryId);
        // check if post IsLiven
        postWithFullLocalizationDto.IsLiven = await PostHelper.IsPostActive(postWithFullLocalizationDto.Id, _postRepository);

        // change DateTime of returned post dto according to user TimeZone
        await PostHelper.ConvertDateTimeToTimeZone(postWithFullLocalizationDto, _userContext, _countryRepository);

        return postWithFullLocalizationDto;
    }
}
