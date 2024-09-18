using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageForAdmin;
public class GetOnboardingPageForAdminQueryValidator : AbstractValidator<GetOnboardingPageForAdminQuery>
{
    private readonly ILogger<GetOnboardingPageForAdminQueryValidator> _logger;
    private readonly IOnboardingPageRepository _onboardingPageRepository;

    public GetOnboardingPageForAdminQueryValidator(ILogger<GetOnboardingPageForAdminQueryValidator> logger,
                                                   IOnboardingPageRepository onboardingPageRepository)
    {
        _logger = logger;
        _onboardingPageRepository = onboardingPageRepository;

        RuleFor(o => o.OnboardingPageId)
            .NotEmpty().WithMessage("Onboarding Page must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsOnboardingPageExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Get Onboarding Page", "Id must be correct");
                }
            });
    }

    public async Task<bool> IsOnboardingPageExisted(GetOnboardingPageForAdminQuery query)
    {
        return await _onboardingPageRepository.GetByIdAsync(query.OnboardingPageId) != null;
    }
}
