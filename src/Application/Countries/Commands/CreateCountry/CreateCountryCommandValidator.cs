using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Countries.Commands.CreateCountry;
public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<CreateCountryCommandValidator> _logger;

    public CreateCountryCommandValidator(ICountryRepository countryRepository,
                                         ILogger<CreateCountryCommandValidator> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;

        RuleFor(c => c.Name)
            .MaximumLength(100).WithMessage("Maximum Length of name is 100 char.")

            .NotEmpty().WithMessage("Country must has Name")

            .MustAsync(BeUniqueName)
              .WithMessage("'{PropertyName}' must be unique.")
              .WithErrorCode("Unique");

        RuleFor(c => c.TimeZoneId)
            .NotEmpty().WithMessage("Country must has Time Zone Id")
            .MustAsync(AcceptableTimeZoneId)
              .WithMessage("'{PropertyName}' must be valid.")
              .WithErrorCode("Invalid");

        RuleFor(c => c.Code)
            .NotEmpty().WithMessage("Country must has Code")
            .MustAsync(BeUniqueCode)
              .WithMessage("'{PropertyName}' must be unique.")
              .WithErrorCode("Unique");
        //TODO : we can validate TimeZoneId according to country name if we restrict entry of name of country
        //from static list in system contains acceptable countries' names


    }

    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _countryRepository.GetAll()
            .AllAsync(c => c.Name != name, cancellationToken);
    }

    public async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return await _countryRepository.GetAll()
            .AllAsync(c => c.Code != code, cancellationToken);
    }

    public async Task<bool> AcceptableTimeZoneId(string timeZoneId, CancellationToken cancellationToken)
    {
        if (!TimeZoneIdentifiers.AcceptableTimeZoneIds.Contains(timeZoneId))
        {
            return await Task.FromResult(false);
        }
        return await Task.FromResult(true);
    }
}
