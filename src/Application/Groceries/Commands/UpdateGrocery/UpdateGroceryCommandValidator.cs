using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
public class UpdateGroceryCommandValidator : AbstractValidator<UpdateGroceryCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<UpdateGroceryCommandValidator> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;

    public UpdateGroceryCommandValidator(IGroceryRepository groceryRepository,
                                         ILogger<UpdateGroceryCommandValidator> logger,
                                         ICountryRepository countryRepository,
                                         ILanguageRepository languageRepository)
    {
        _groceryRepository = groceryRepository;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
        RuleFor(g => g.Name)
            .NotEmpty().WithMessage("Grocery must has Name")

            .MaximumLength(200).WithMessage("Maximum Length of name is 200 char.")

            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueName(context.InstanceToValidate))
                {
                    context.AddFailure("Name", "Name must be unique.");
                }
            });

        RuleFor(g => g.Address)
            .NotEmpty().WithMessage("Grocery must has Address");

        RuleFor(g => g.Description)
            .NotEmpty().WithMessage("Grocery must has Description");

        RuleFor(g => g.Id)
            .NotEmpty().WithMessage("Grocery must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroceryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update Grocery", "GroceryId must be correct");
                }
            });

        RuleFor(g => g.CountryId)
            .NotEmpty().WithMessage("Country must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update Grocery", "Country is not found");
                }
            });

        RuleFor(g => g.GroceryLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroceryLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Update Grocery", "Grocery Localizations has invalid or empty values");
                }
            });
    }

    public async Task<bool> BeUniqueName(UpdateGroceryCommand command)
    {
        return !await _groceryRepository.GetAll()
             .AnyAsync(l => l.Name == command.Name && l.Id != command.Id);
    }

    public async Task<bool> IsGroceryExisted(UpdateGroceryCommand command)
    {
        return await _groceryRepository.GetByIdAsync(command.Id) != null;
    }

    public async Task<bool> IsCountryExisted(UpdateGroceryCommand command)
    {
        return await _countryRepository.GetByIdAsync(command.CountryId) != null;
    }

    public async Task<bool> IsGroceryLocalizationsValid(UpdateGroceryCommand command)
    {
        List<Language> languages;
        var query = _languageRepository.GetAll();
        if (query.IsEntityFrameworkQueryable())
            languages = await query.ToListAsync();
        else
            languages = query.ToList();
        foreach (var groceryLocalization in command.GroceryLocalizations)
        {
            if (!languages.Any(l => l.Id == groceryLocalization.LanguageId))
            {
                return false;
            }
            if (groceryLocalization.Value == string.Empty)
            {
                return false;
            }
        }
        return true;
    }
}
