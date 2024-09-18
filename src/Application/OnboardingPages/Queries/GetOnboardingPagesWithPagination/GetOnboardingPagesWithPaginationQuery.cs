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
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPagesWithPagination;
public class GetOnboardingPagesWithPaginationQuery : IRequest<PaginatedList<GetOnboardingPagesWithPaginationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public Guid? LanguageId { get; set; }

}

public class GetOnboardingPagesWithPaginationQueryHandler : IRequestHandler<GetOnboardingPagesWithPaginationQuery, PaginatedList<GetOnboardingPagesWithPaginationDto>>
{
    private readonly ILogger<GetOnboardingPagesWithPaginationQueryHandler> _logger;
    private readonly IOnboardingPageRepository _onboardingPageRepository;
    private readonly IOnboardingPageLocalizationRepository _onboardingPageLocalizationRepository;
    private readonly IUserContext _userContext;
    private readonly ILanguageRepository _languageRepository;
    private readonly IMapper _mapper;

    public GetOnboardingPagesWithPaginationQueryHandler(ILogger<GetOnboardingPagesWithPaginationQueryHandler> logger,
                                                        IOnboardingPageRepository onboardingPageRepository,
                                                        IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository,
                                                        IUserContext userContext,
                                                        ILanguageRepository languageRepository,
                                                        IMapper mapper)
    {
        _logger = logger;
        _onboardingPageRepository = onboardingPageRepository;
        _onboardingPageLocalizationRepository = onboardingPageLocalizationRepository;
        _userContext = userContext;
        _languageRepository = languageRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetOnboardingPagesWithPaginationDto>> Handle(GetOnboardingPagesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // check if user has token

        Guid userLanguageId = Guid.Empty;
        if (_userContext.CheckIfUserAuthorized())// get user language from token
        {
            userLanguageId = _userContext.GetLanguageIdOfUser();
        }
        else
        {
            // here we care about user language

            if (request.LanguageId != null)
            {
                // convert nullable LanguageId to not nullable
                userLanguageId = NullableValuesHelper.ConvertNullableGuid(request.LanguageId);
            }
        }

        // get Onboarding Pages

        var onboardingPages = _onboardingPageRepository.GetAll();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            onboardingPages = onboardingPages.Where(p => p.Title.ToLower().Contains(request.SearchText.ToLower()));
        }

        var result = await onboardingPages
            .OrderBy(o => o.Order)
            .ProjectTo<GetOnboardingPagesWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // fill localization according to user language, if don't have language, we return standard
        if (userLanguageId != Guid.Empty)
            await LocalizationHelper.FillOnboardingPageLocalizations(result,userLanguageId,_onboardingPageLocalizationRepository);
        
        return result;
    }
}
