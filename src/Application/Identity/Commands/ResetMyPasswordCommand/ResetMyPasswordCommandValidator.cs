using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.Identity.Commands.ResetMyPasswordCommand;
public class ResetMyPasswordCommandValidator : AbstractValidator<ResetMyPasswordCommand>
{
    private StringBuilder sb = new StringBuilder();
    private readonly IIdentityService _identityService;
    private readonly IApplicationPasswordValidator _applicationPasswordValidator;
    private readonly ILogger<ResetMyPasswordCommandValidator> _logger;

    public ResetMyPasswordCommandValidator(IIdentityService identityService,
                                           IApplicationPasswordValidator applicationPasswordValidator,
                                           ILogger<ResetMyPasswordCommandValidator> logger)
    {
        _identityService = identityService;
        _applicationPasswordValidator = applicationPasswordValidator;
        _logger = logger;
        RuleFor(p=>p.UserId)
            .NotEmpty().WithMessage("User Id is required")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Reset Password", "User is not found");
                }
            });

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("Password is required")
            .Equal(p => p.PasswordConfirmation)
            .WithMessage("Password must match the password confirmation")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsPsswordValid(context.InstanceToValidate))
                {
                    context.AddFailure("Reset Password", sb.ToString());
                }
            });

    }

    public async Task<bool> IsUserExisted(ResetMyPasswordCommand command)
    {
        var user = await _identityService.GetUserByIdAsync(command.UserId);
        if (user == null)
            return false;
        return true;
    }

    public async Task<bool> IsPsswordValid(ResetMyPasswordCommand command)
    {
        var res = await _applicationPasswordValidator.ValidatePassword(command.UserId, command.Password);
        if (!res.Succeeded)
        {
            foreach (string error in res.Errors)
            {
                sb.AppendLine(error);
            }
            return false;
        }   
        return true;
    }
}
