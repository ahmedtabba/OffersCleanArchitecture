using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Countries.Commands.UpdateCountry;
public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<UpdateCountryCommandValidator> _logger;

    public UpdateCountryCommandValidator(ICountryRepository countryRepository,
                                         ILogger<UpdateCountryCommandValidator> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;

        RuleFor(c => c.Id)
           .NotEmpty().WithMessage("Country must has Id")
           .CustomAsync(async (name, context, cancellationToken) =>
           {
               if (!await IsCountryExisted(context.InstanceToValidate))
               {
                   context.AddFailure("Update Country", "CountryId must be correct");
               }
           });


        RuleFor(c => c.Name)
            .MaximumLength(100).WithMessage("Maximum Length of name is 100 char.")

            .NotEmpty().WithMessage("Country must has Name")

            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueName(context.InstanceToValidate))
                {
                    context.AddFailure("Name", "Name must be unique.");
                }
            });

        RuleFor(c => c.TimeZoneId)
            .NotEmpty().WithMessage("Country must has Time Zone Id")
            .MustAsync(AcceptableTimeZoneId)
              .WithMessage("'{PropertyName}' must be valid.")
              .WithErrorCode("Invalid");

        RuleFor(c => c.Code)
            .NotEmpty().WithMessage("Country must has Code")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueCode(context.InstanceToValidate))
                {
                    context.AddFailure("Code", "Code must be unique.");
                }
            });
    }

    public async Task<bool> BeUniqueName(UpdateCountryCommand command)
    {
        return !await _countryRepository.GetAll()
             .AnyAsync(l => l.Name == command.Name && l.Id != command.Id);
    }

    public async Task<bool> BeUniqueCode(UpdateCountryCommand command)
    {
        return !await _countryRepository.GetAll()
             .AnyAsync(l => l.Code == command.Code && l.Id != command.Id);
    }

    public async Task<bool> IsCountryExisted(UpdateCountryCommand command)
    {
        return await _countryRepository.GetByIdAsync(command.Id) != null;
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
