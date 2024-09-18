using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Commands.DeleteOnboardingPage;
public class DeleteOnboardingPageCommandValidator : AbstractValidator<DeleteOnboardingPageCommand>
{
    private readonly ILogger<DeleteOnboardingPageCommandValidator> _logger;
    private readonly IOnboardingPageRepository _onboardingPageRepository;

    public DeleteOnboardingPageCommandValidator(ILogger<DeleteOnboardingPageCommandValidator> logger,
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
                    context.AddFailure("Delete Onboarding Page", "Onboarding Page is not found");
                }
            });
    }

    public async Task<bool> IsOnboardingPageExisted(DeleteOnboardingPageCommand command)
    {
        return await _onboardingPageRepository.GetByIdAsync(command.OnboardingPageId) != null;
    }
}
