using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Identity.Commands.SignUp;
public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    private StringBuilder sb = new StringBuilder();
    private readonly ILogger<SignUpCommandValidator> _logger;
    private readonly IIdentityService _identityService;
    private readonly IApplicationPasswordValidator _applicationPasswordValidator;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;

    public SignUpCommandValidator(ILogger<SignUpCommandValidator> logger,
                                  IIdentityService identityService,
                                  IApplicationPasswordValidator applicationPasswordValidator,
                                  ICountryRepository countryRepository,
                                  ILanguageRepository languageRepository)
    {
        _logger = logger;
        _identityService = identityService;
        _applicationPasswordValidator = applicationPasswordValidator;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsEmailAddressValid(context.InstanceToValidate))
                {
                    context.AddFailure("Sign Up", "Email address is already existed");
                }
            });

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsPasswordValid(context.InstanceToValidate))
                {
                    context.AddFailure("Password", sb.ToString());
                }
            });

        RuleFor(u => u.ConfirmedPassword)
            .NotEmpty().WithMessage("Confirm Password is required")
            .Equal(u => u.Password).WithMessage("Confirmed Password dosn't match Password");

        RuleFor(u => u.CountryId)
            .NotEmpty().WithMessage("Country Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Sign Up", "Country is not found");
                }
            });

        RuleFor(u => u.LanguageId)
            .NotEmpty().WithMessage("Language Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Sign Up", "Language is not found");
                }
            });
    }

    public async Task<bool> IsEmailAddressValid(SignUpCommand command)
    {
        var user = await _identityService.GetUserByEmailAsync(command.Email);
        if (user != null)
            return false;
        return true;
    }

    public async Task<bool> IsPasswordValid(SignUpCommand command)
    {
        var res = await _applicationPasswordValidator.ValidatePassword("", command.Password);
        if (!res.Succeeded)
        {
            foreach (string error in res.Errors)
            {
                sb.AppendLine(error);
            }
            return false;
        }
        return true;
    }

    public async Task<bool> IsCountryExisted(SignUpCommand command)
    {
        var country = await _countryRepository.GetByIdAsync(command.CountryId);
        if (country is null)
            return false;
        return true;
    }

    public async Task<bool> IsLanguageExisted(SignUpCommand command)
    {
        var language = await _languageRepository.GetByIdAsync(command.LanguageId);
        if (language is null)
            return false;
        return true;
    }
}
