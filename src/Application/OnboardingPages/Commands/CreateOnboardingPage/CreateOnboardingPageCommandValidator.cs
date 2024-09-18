using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Commands.CreateOnboardingPage;
public class CreateOnboardingPageCommandValidator : AbstractValidator<CreateOnboardingPageCommand>
{
    private readonly ILogger<CreateOnboardingPageCommandValidator> _logger;
    private readonly IOnboardingPageRepository _onboardingPageRepository;
    private readonly ILanguageRepository _languageRepository;

    public CreateOnboardingPageCommandValidator(ILogger<CreateOnboardingPageCommandValidator> logger,
                                                IOnboardingPageRepository onboardingPageRepository,
                                                ILanguageRepository languageRepository)
    {
        _logger = logger;
        _onboardingPageRepository = onboardingPageRepository;
        _languageRepository = languageRepository;

        RuleFor(o => o.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Onboarding Page must has order")
            //.MustAsync(BeUniqueOrder)
            //.WithMessage("'{PropertyName}' must be unique.")
            //.WithErrorCode("Unique");
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueOrder(context.InstanceToValidate))
                {
                    context.AddFailure("Create Onboarding Page", "Order must be unique");
                }
            });
        RuleFor(o => o.Title)
            .NotEmpty().WithMessage("Onboarding Page must has title")
            .MaximumLength(20).WithMessage("Onboarding Page title length must be 20 character at maximum");

        RuleFor(o => o.Description)
            .NotEmpty().WithMessage("Onboarding Page must has Description")
            .MaximumLength(55).WithMessage("Onboarding Page Description length must be 55 character at maximum");

        RuleFor(o => o.Asset)
            .NotEmpty().WithMessage("Onboarding Page must has one photo");

        RuleFor(o => o.OnboardingPageLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create Onboarding Page", "Localizations have invalid or empty values");
                }
            });

        RuleFor(o => o.OnboardingPageLocalizationAssets)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizationsAssetLanguageValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create Onboarding Page", "Localizations asset have invalid language values");
                }
            })
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizationsAssetHaveRepeatedValues(context.InstanceToValidate))
                {
                    context.AddFailure("Create Onboarding Page", "Localizations asset have repeated values");
                }
            });

    }

    public async Task<bool> BeUniqueOrder(CreateOnboardingPageCommand command)
    {
        return await _onboardingPageRepository.GetAll()
            .AllAsync(c => c.Order != command.Order);
    }

    public async Task<bool> AreOnboardingPageLocalizationsValid(CreateOnboardingPageCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages= await query.ToListAsync();
        else
            languages =query.ToList();


        foreach (var localization in command.OnboardingPageLocalizations)
        {
            if (!languages.Any(l => l.Id == localization.LanguageId))
            {
                return false;
            }
            if (localization.Value == string.Empty)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> AreOnboardingPageLocalizationsAssetLanguageValid(CreateOnboardingPageCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages = await query.ToListAsync();
        else
            languages = query.ToList();
        foreach (var localizationAsset in command.OnboardingPageLocalizationAssets)
        {
            if (!languages.Any(l => l.Id == localizationAsset.LanguageId))
            {
                return false;
            }
        }
        return true;
    }
    public async Task<bool> AreOnboardingPageLocalizationsAssetHaveRepeatedValues(CreateOnboardingPageCommand command)
    {
        if (command.OnboardingPageLocalizationAssets.DistinctBy(l => l.LanguageId).Count() != command.OnboardingPageLocalizationAssets.Count)
        {
            return await Task.FromResult(false);
        }
        return await Task.FromResult(true);
    }
}
