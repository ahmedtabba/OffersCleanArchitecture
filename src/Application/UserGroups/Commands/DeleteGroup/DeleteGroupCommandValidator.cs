using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.UserGroups.Commands.DeleteGroup;
public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<DeleteGroupCommandValidator> _logger;

    public DeleteGroupCommandValidator(IApplicationGroupManager applicationGroupManager, ILogger<DeleteGroupCommandValidator> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
        RuleFor(g => g.groupId)
            .NotEmpty().WithMessage("Group Id must passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsGroupExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Delete Group", "Group is not found");
                }
            });

    }

    public async Task<bool> IsGroupExisted(DeleteGroupCommand command)
    {
        var group = await _applicationGroupManager.FindByIdAsync(command.groupId);
        if (group is null)
            return false;
        return true;

    }
}
