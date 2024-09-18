using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;
using Offers.CleanArchitecture.Application.Identity.Commands.Login;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Offers.CleanArchitecture.Application.Identity.Commands.DeleteUser;
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user;
    private readonly ILogger<DeleteUserCommandValidator> _logger;

    public DeleteUserCommandValidator(IIdentityService identityService,
                                      IUser user,
                                      ILogger<DeleteUserCommandValidator> logger)
    {
        _identityService = identityService;
        _user = user;
        _logger = logger;
        RuleFor(u => u.UserId)
           .NotEmpty().WithMessage("UserId cann't be empty")
           .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Delete User", "User isn't found");
                }
            })
           .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (await IsUserDeleteItsSelf(context.InstanceToValidate))
                {
                    context.AddFailure("Delete User", "You can't delete your self");
                }
            });

    }
    public async Task<bool> IsUserExisted(DeleteUserCommand command)
    {
        // Check if user is null
        var user = await _identityService.GetUserByIdAsync(command.UserId);
        if (user is null)
            return false;
        return true;

    }

    public async Task<bool> IsUserDeleteItsSelf(DeleteUserCommand command)
    {
        if (_user.Id != command.UserId)
            return await Task.FromResult(false);
        return await Task.FromResult(true);

    }
}
