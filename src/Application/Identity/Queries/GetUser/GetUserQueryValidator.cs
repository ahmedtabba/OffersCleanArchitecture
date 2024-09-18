using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.Identity.Queries.GetUser;
public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<GetUserQueryValidator> _logger;

    public GetUserQueryValidator(IIdentityService identityService, ILogger<GetUserQueryValidator> logger)
    {
        _identityService = identityService;
        _logger = logger;
        RuleFor(u => u.userId)
            .NotEmpty().WithMessage("User Id must passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Get User", "User isn't found");
                }
            });
    }

    public async Task<bool> IsUserExisted(GetUserQuery query)
    {
        // Check if user is null
        var user = await _identityService.GetUserByIdAsync(query.userId);
        if (user is null)
            return false;
        return true;

    }
}
