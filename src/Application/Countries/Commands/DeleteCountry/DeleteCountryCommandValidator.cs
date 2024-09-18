using System.Numerics;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.Countries.Commands.DeleteCountry;

public class DeleteCountryCommandValidator : AbstractValidator<DeleteCountryCommand>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<DeleteCountryCommandValidator> _logger;
    private readonly IIdentityService _identityService;

    public DeleteCountryCommandValidator(ICountryRepository countryRepository,
                                         ILogger<DeleteCountryCommandValidator> logger,
                                         IIdentityService identityService)
    {
        _countryRepository = countryRepository;
        _logger = logger;
        _identityService = identityService;
        RuleFor(c => c.CountryId)

       .NotEmpty().WithMessage("Id Must be passed")
       .CustomAsync(async (name, context, cancellationToken) =>
       {
           if (!await CanDeleteCountry(context.InstanceToValidate))
           {
               context.AddFailure("Delete Country", "Country is not found or has Groceries or Users!");
           }
       });

    }

    public async Task<bool> CanDeleteCountry(DeleteCountryCommand command)
    {
        var country = await _countryRepository.GetCountryWithGroceriesByCountryIdAsync(command.CountryId);

        if (country is null || country.Groceries.Any() /*|| country.UsersIds.Any()*/ )
            return false;
        else
        {
            var AreThereUsersOfCountry = await _identityService.GetAllUsers()
                .AnyAsync(u => u.CountryId == command.CountryId.ToString());
            if (AreThereUsersOfCountry)
                return false;
        }
        return true;
    }
}
