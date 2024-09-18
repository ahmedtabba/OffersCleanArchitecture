using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
public class CreateGroceryCommandValidator : AbstractValidator<CreateGroceryCommand>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<CreateGroceryCommandValidator> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly ILanguageRepository _languageRepository;

    public CreateGroceryCommandValidator(IGroceryRepository groceryRepository,
                                         ILogger<CreateGroceryCommandValidator> logger,
                                         ICountryRepository countryRepository,
                                         ILanguageRepository languageRepository)
    {
        _groceryRepository = groceryRepository;
        _logger = logger;
        _countryRepository = countryRepository;
        _languageRepository = languageRepository;
        RuleFor(g => g.Name)
            .MaximumLength(200).WithMessage("Maximum Length of name is 200 char.")

            .NotEmpty().WithMessage("Grocery must has Name")

            .MustAsync(BeUniqueName)
              .WithMessage("'{PropertyName}' must be unique.")
              .WithErrorCode("Unique");

        RuleFor(g => g.Address)
            .NotEmpty().WithMessage("Grocery must has Address");

        RuleFor(g => g.Description)
            .NotEmpty().WithMessage("Grocery must has Description");

        RuleFor(c => c.File)
            .NotEmpty().WithMessage("Grocery must has one photo");

        RuleFor(g => g.CountryId)
            .NotEmpty().WithMessage("CountryId must passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Create Grocery", "Country is not found");
                }
            });

        RuleFor(g => g.GroceryLocalizations)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreGroceryLocalizationsValid(context.InstanceToValidate))
                {
                    context.AddFailure("Create Grocery", "Grocery Localizations have invalid or empty values");
                }
            });
    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _groceryRepository.GetAll()
            .AllAsync(c => c.Name != name, cancellationToken);
    }

    public async Task<bool> IsCountryExisted(CreateGroceryCommand command)
    {
        return await _countryRepository.GetByIdAsync(command.CountryId) != null;
    }

    public async Task<bool> AreGroceryLocalizationsValid(CreateGroceryCommand command)
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
