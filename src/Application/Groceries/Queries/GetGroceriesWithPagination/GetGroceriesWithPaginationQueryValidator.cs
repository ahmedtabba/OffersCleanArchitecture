using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.AddToFavoraite;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
public class GetGroceriesWithPaginationQueryValidator : AbstractValidator<GetGroceriesWithPaginationQuery>
{
    private readonly ILogger<GetGroceriesWithPaginationQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICountryRepository _countryRepository;

    public GetGroceriesWithPaginationQueryValidator(ILogger<GetGroceriesWithPaginationQueryValidator> logger,
                                                    ILanguageRepository languageRepository,
                                                    ICountryRepository countryRepository)
    {
        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");

        RuleFor(x => x.LanguageId)
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsLanguageIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("LanguageId", "LanguageId must be correct");
                }
            });

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country must be chosen or Id must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsCountryIdParamAcceptable(context.InstanceToValidate))
                {
                    context.AddFailure("CountryId", "CountryId must be correct");
                }
            });

        _logger = logger;
        _languageRepository = languageRepository;
        _countryRepository = countryRepository;
    }
    public async Task<bool> IsLanguageIdParamAcceptable(GetGroceriesWithPaginationQuery query)
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

    public async Task<bool> IsCountryIdParamAcceptable(GetGroceriesWithPaginationQuery query)
    {
        // CountryId is acceptable if it is valid
        return await _countryRepository.GetByIdAsync(query.CountryId) != null;
    }
}
