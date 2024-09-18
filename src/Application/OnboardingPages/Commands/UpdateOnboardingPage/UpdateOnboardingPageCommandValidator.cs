using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Posts.Commands.UpdatePost;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;


namespace Offers.CleanArchitecture.Application.OnboardingPages.Commands.UpdateOnboardingPage;
public class UpdateOnboardingPageCommandValidator : AbstractValidator<UpdateOnboardingPageCommand>
{
    private readonly ILogger<UpdateOnboardingPageCommandValidator> _logger;
    private readonly IOnboardingPageRepository _onboardingPageRepository;
    private readonly IOnboardingPageLocalizationRepository _onboardingPageLocalizationRepository;
    private readonly ILanguageRepository _languageRepository;

    public UpdateOnboardingPageCommandValidator(ILogger<UpdateOnboardingPageCommandValidator> logger,
                                                IOnboardingPageRepository onboardingPageRepository,
                                                IOnboardingPageLocalizationRepository onboardingPageLocalizationRepository,
                                                ILanguageRepository languageRepository)
    {
        _logger = logger;
        _onboardingPageRepository = onboardingPageRepository;
        _onboardingPageLocalizationRepository = onboardingPageLocalizationRepository;
        _languageRepository = languageRepository;

        RuleFor(o => o.Title)
            .NotEmpty().WithMessage("Onboarding Page must has title")
            .MaximumLength(20).WithMessage("Onboarding Page title length must be 20 character at maximum");

        RuleFor(o => o.Description)
            .NotEmpty().WithMessage("Onboarding Page must has Description")
            .MaximumLength(55).WithMessage("Onboarding Page Description length must be 55 character at maximum");

        RuleFor(o => o.Id)
            .NotEmpty().WithMessage("Onboarding Page must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsOnboardingPageExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update Onboarding Page", "Id must be correct");
                }
            });

        RuleFor(o => o.Order)
            .GreaterThanOrEqualTo(0).WithMessage("Onboarding Page must has order")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsOnboardingPageOrderValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Onboarding Page", "Order is invalid");
                }
            });

        RuleFor(o => o.OnboardingPageLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Onboarding Page", "Localizations have invalid or empty values");
                }
            });

        RuleFor(o => o.OnboardingPageLocalizationAssets)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizationsAssetLanguageValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Onboarding Page", "Localizations asset have invalid language values");
                }
            })
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizationsAssetHaveRepeatedValues(context.InstanceToValidate))
                {
                    context.AddFailure("Update Onboarding Page", "Localizations asset have repeated values");
                }
            });

        RuleFor(o => o.DeletedLocalizedAssetsIds)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreOnboardingPageLocalizedAssetsIdsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Onboarding Page", "Old Localizations asset are not found or not AssetPath type");
                }
            });

    }
    public async Task<bool> IsOnboardingPageExisted(UpdateOnboardingPageCommand command)
    {
        return await _onboardingPageRepository.GetByIdAsync(command.Id) != null;
    }

    public async Task<bool> IsOnboardingPageOrderValid(UpdateOnboardingPageCommand command)
    {
        return !await _onboardingPageRepository.GetAll()
            .AnyAsync(o => o.Id != command.Id && o.Order == command.Order);
    }

    public async Task<bool> AreOnboardingPageLocalizationsValid(UpdateOnboardingPageCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages = await query.ToListAsync();
        else
            languages = query.ToList();
        foreach (var onboardingPageLocalization in command.OnboardingPageLocalizations)
        {
            if (!languages.Any(l => l.Id == onboardingPageLocalization.LanguageId))
            {
                return false;
            }
            if (onboardingPageLocalization.Value == string.Empty)
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> AreOnboardingPageLocalizationsAssetLanguageValid(UpdateOnboardingPageCommand command)
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
    public async Task<bool> AreOnboardingPageLocalizationsAssetHaveRepeatedValues(UpdateOnboardingPageCommand command)
    {
        if (command.OnboardingPageLocalizationAssets.DistinctBy(l => l.LanguageId).Count() != command.OnboardingPageLocalizationAssets.Count)
        {
            return await Task.FromResult(false);
        }
        return await Task.FromResult(true);
    }

    public async Task<bool> AreOnboardingPageLocalizedAssetsIdsValid(UpdateOnboardingPageCommand command)
    {
        foreach (var id in command.DeletedLocalizedAssetsIds)
        {
            var onboardingPageLocalizationAsset = await _onboardingPageLocalizationRepository.GetByIdAsync(id);
            if (onboardingPageLocalizationAsset == null 
                || onboardingPageLocalizationAsset.OnboardingPageLocalizationFieldType != (int)OnboardingPageLocalizationFieldType.AssetPath)
                return false;
        }
        return true;

    }
}
