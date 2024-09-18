using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageForAdmin;
public class GetOnboardingPageForAdminQuery : IRequest<GetOnboardingPageForAdminQueryDto> // this get query is for admin only because usual user should get all for just one time 
{
    public Guid OnboardingPageId { get; set; }
}

public class GetOnboardingPageQueryHandler : IRequestHandler<GetOnboardingPageForAdminQuery, GetOnboardingPageForAdminQueryDto>
{
    private readonly ILogger<GetOnboardingPageQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IOnboardingPageRepository _onboardingPageRepository;
    private readonly IOnboardingPageLocalizationRepository _onboardingPageLocalizationRepository;

    public GetOnboardingPageQueryHandler(ILogger<GetOnboardingPageQueryHandler> logger,
                                         IMapper mapper,
                                         IOnboardingPageRepository onboardingPageRepository,
                                         IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _onboardingPageRepository = onboardingPageRepository;
        _onboardingPageLocalizationRepository = onboardingPageLocalizationRepository;
    }

    public async Task<GetOnboardingPageForAdminQueryDto> Handle(GetOnboardingPageForAdminQuery request, CancellationToken cancellationToken)
    {
        // get Onboarding Page from DB
        var onboardingPage = await _onboardingPageRepository.GetByIdAsync(request.OnboardingPageId);
        // mapping , but without localization
        var onboardingPageWithFullLocalizationDto = _mapper.Map<GetOnboardingPageForAdminQueryDto>(onboardingPage);
        //fill localization 
        await LocalizationHelper.FillOnboardingPageLocalizations(onboardingPageWithFullLocalizationDto, _onboardingPageLocalizationRepository, _mapper);

        return onboardingPageWithFullLocalizationDto;

    }
}
