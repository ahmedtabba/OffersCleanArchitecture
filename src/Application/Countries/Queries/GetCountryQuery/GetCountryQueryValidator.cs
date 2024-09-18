using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Countries.Queries.GetCountryQuery;
public class GetCountryQueryValidator : AbstractValidator<GetCountryQuery>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<GetCountryQueryValidator> _logger;

    public GetCountryQueryValidator(ICountryRepository countryRepository,
                                    ILogger<GetCountryQueryValidator> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;

        RuleFor(c => c.CountryId)
           .NotEmpty().WithMessage("Country must has Id")
           .CustomAsync(async (name, context, cancellationToken) =>
           {
               if (!await IsCountryExisted(context.InstanceToValidate))
               {
                   context.AddFailure("Get Country", "CountryId must be correct");
               }
           });
    }

    public async Task<bool> IsCountryExisted(GetCountryQuery query)
    {
        return await _countryRepository.GetByIdAsync(query.CountryId) != null;
    }
}
