using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
public class GetPostsByGroceryWithPaginationQueryValidator : AbstractValidator<GetPostsByGroceryWithPaginationQuery>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<GetPostsByGroceryWithPaginationQueryValidator> _logger;
    private readonly ILanguageRepository _languageRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;

    public GetPostsByGroceryWithPaginationQueryValidator(IGroceryRepository groceryRepository,
                                                         ILogger<GetPostsByGroceryWithPaginationQueryValidator> logger,
                                                         ILanguageRepository languageRepository,
                                                         ICountryRepository countryRepository)
    {
        _groceryRepository = groceryRepository;
        _logger = logger;
        _languageRepository = languageRepository;
        _countryRepository = countryRepository;
        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");

        RuleFor(x=>x.GroceryId)
            .NotEmpty().WithMessage("Grocery Id should passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroceryExisted(context.InstanceToValidate))
                {
                    context.AddFailure("GroceryId", "GroceryId must be correct");
                }
            });

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

    }
    public async Task<bool> IsGroceryExisted(GetPostsByGroceryWithPaginationQuery query)
    {
        return await _groceryRepository.GetByIdAsync(query.GroceryId) != null;
    }

    public async Task<bool> IsLanguageIdParamAcceptable(GetPostsByGroceryWithPaginationQuery query)
    {
        // LanguageId is acceptable if it is Empty or valid
        if (query.LanguageId == Guid.Empty)
        {
            return true;
        }
        else
        {//check if LanguageId valid
            return await _languageRepository.GetByIdAsync(query.LanguageId) != null;
        }
    }

    public async Task<bool> IsCountryIdParamAcceptable(GetPostsByGroceryWithPaginationQuery query)
    {
        // CountryId is acceptable if it is valid
        if (query.CountryId == Guid.Empty)
        {
            return false;
        }
        else
        {//check if CountryId valid
            return await _countryRepository.GetByIdAsync(query.CountryId) != null;
        }
    }
}
