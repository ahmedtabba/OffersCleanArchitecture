using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Commands.UpdatePost;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostQuery;
public class GetPostQuery : IRequest<GetPostDto>
{
    public Guid postId { get; set; }
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
}

public class GetPostQueryHandler : IRequestHandler<GetPostQuery, GetPostDto>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _postRepository;
    private readonly ILogger<GetPostQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IPostLocalizationRepository _postLocalizationRepository;
    private readonly IGroceryRepository _groceryRepository;
    private readonly ICountryRepository _countryRepository;

    public GetPostQueryHandler(IMapper mapper,
                               IPostRepository postRepository,
                               ILogger<GetPostQueryHandler> logger,
                               IUserContext userContext,
                               IPostLocalizationRepository postLocalizationRepository,
                               IGroceryRepository groceryRepository,
                               ICountryRepository countryRepository)
    {
        _mapper = mapper;
        _postRepository = postRepository;
        _logger = logger;
        _userContext = userContext;
        _postLocalizationRepository = postLocalizationRepository;
        _groceryRepository = groceryRepository;
        _countryRepository = countryRepository;
    }
    public async Task<GetPostDto> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        // get post from DB
        var post = await _postRepository.GetByIdAsync(request.postId);

        // mapping , but without GroceryName or IsLiven
        var postDto = _mapper.Map<GetPostDto>(post);

        // change post localization according to user language
        if (request.LanguageId != Guid.Empty)
            await LocalizationHelper.FillPostLocalizations(postDto, request.LanguageId, _postLocalizationRepository);

        // fill GroceryName and check if post IsLiven
        postDto.GroceryName = await _groceryRepository.GetGroceryNameByGroceryIdAsync(postDto.GroceryId);
        postDto.IsLiven = await PostHelper.IsPostActive(postDto.Id, _postRepository);

        // change DateTime of returned post dto according to user TimeZone
        await PostHelper.ConvertDateTimeToTimeZone(postDto, /*_userContext,*/ _countryRepository,request.CountryId);
        
        return postDto;
    }
}
