using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.UserGroups.Commands.UpdateGroup;
public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<UpdateGroupCommandValidator> _logger;

    public UpdateGroupCommandValidator(IApplicationGroupManager applicationGroupManager, ILogger<UpdateGroupCommandValidator> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
        RuleFor(g => g.GroupId)
             .NotEmpty().WithMessage("Group must has Id")
             .CustomAsync(async (name, context, cancellationToken) =>
             {
                 if (!await IsGroupExisted(context.InstanceToValidate))
                 {
                     context.AddFailure("Update Group", "Group is not found");
                 }
             });

        RuleFor(g => g.Name)
             .NotEmpty().WithMessage("Group must has Name");

        RuleFor(g => g.Description)
             .NotEmpty().WithMessage("Group must has Description");

        RuleFor(g => g.Roles)
             .NotEmpty().WithMessage("RolesIds must have values")
             .CustomAsync(async (name, context, cancellationToken) =>
             {
                 if (!await AreRolesIdsValid(context.InstanceToValidate))
                 {
                     context.AddFailure("Update Group", "Some Roles Ids are not valid");
                 }
             });

    }

    public async Task<bool> IsGroupExisted(UpdateGroupCommand command)
    {
        var group = await _applicationGroupManager.FindByIdAsync(command.GroupId);
        if (group is null)
            return false;
        return true;
    }

    public async Task<bool> AreRolesIdsValid(UpdateGroupCommand command)
    {
        var groupRols = await _applicationGroupManager.GetAllRoles();
        foreach (var roleId in command.Roles)
        {
            if (!groupRols.Any(r=>r.Id == roleId))
            {
                return false;
            }
        }
        return true;
    }
}
