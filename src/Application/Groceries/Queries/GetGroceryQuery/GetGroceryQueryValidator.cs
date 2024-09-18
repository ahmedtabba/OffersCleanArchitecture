using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
public class GetGroceryQueryValidator : AbstractValidator<GetGroceryQuery>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<GetGroceryQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICountryRepository _countryRepository;

    public GetGroceryQueryValidator(IGroceryRepository groceryRepository,
                                    ILogger<GetGroceryQueryValidator> logger,
                                    ILanguageRepository languageRepository,
                                    ICountryRepository countryRepository)
    {
        _groceryRepository = groceryRepository;
        _logger = logger;
        _languageRepository = languageRepository;
        _countryRepository = countryRepository;

        RuleFor(g => g.GroceryId)
              .NotEmpty().WithMessage("Grocery Id should passed")
              .CustomAsync(async (name, context, cancellationToken) =>
              {
                  if (!await IsGroceryExisted(context.InstanceToValidate))
                  {
                      context.AddFailure("Get Grocery", "GroceryId must be correct");
                  }
              });

        RuleFor(g => g.LanguageId)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("LanguageId", "LanguageId must be correct");
                }
            });

        RuleFor(g => g.CountryId)
            .NotEmpty().WithMessage("Country must be chosen or Id must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("CountryId", "CountryId must be correct");
                }
            });

    }
    public async Task<bool> IsGroceryExisted(GetGroceryQuery query)
    {
        return await _groceryRepository.GetByIdAsync(query.GroceryId) != null;
    }

    public async Task<bool> IsLanguageIdParamAcceptable(GetGroceryQuery query)
    {
        // LanguageId is acceptable if it is null (Guid.Empty) or valid
        if (query.LanguageId == Guid.Empty)
        {
            return true;
        }
        else
        {//check if LanguageId valid
            return await _languageRepository.GetByIdAsync(query.LanguageId) != null;
        }
    }

    public async Task<bool> IsCountryIdParamAcceptable(GetGroceryQuery query)
    {
        // CountryId is acceptable if it is equal to grocery's country
        if (query.CountryId == Guid.Empty)
        {
            return await Task.FromResult(false);
        }
        else
        {//check if CountryId valid
            return  _groceryRepository.GetByIdAsync(query.GroceryId).Result.CountryId ==  _countryRepository.GetByIdAsync(query.CountryId).Result.Id;
        }
    }
}
