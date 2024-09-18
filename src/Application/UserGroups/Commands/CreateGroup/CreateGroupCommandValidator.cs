using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.UserGroups.Commands.CreateGroup;
public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<CreateGroupCommandValidator> _logger;

    public CreateGroupCommandValidator(IApplicationGroupManager applicationGroupManager,
                                        ILogger<CreateGroupCommandValidator> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
        RuleFor(g => g.Name)
             .NotEmpty().WithMessage("Group must has Name")
             .CustomAsync(async (name, context, cancellationToken) =>
             {
                 if (!await IsGroupNameValid(context.InstanceToValidate))
                 {
                     context.AddFailure("Create Group", "Group name is not valid");
                 }
             });

        RuleFor(g => g.Description)
             .NotEmpty().WithMessage("Group must has Description");


        RuleFor(g => g.RolesIds)
             .NotEmpty().WithMessage("RolesIds must have values")
             .CustomAsync(async (name, context, cancellationToken) =>
             {
                 if (!await AreRolesIdsValid(context.InstanceToValidate))
                 {
                     context.AddFailure("Create Group", "Some Roles Ids are not valid");
                 }
             });

    }

    public async Task<bool> AreRolesIdsValid(CreateGroupCommand command)
    {
        var groupRols = await _applicationGroupManager.GetAllRoles();
        foreach (var roleId in command.RolesIds)
        {
            if (!groupRols.Any(r => r.Id == roleId))
            {
                return false;
            }
        }
        return true;
    }

    public async Task<bool> IsGroupNameValid(CreateGroupCommand command)
    {
        var group = await _applicationGroupManager.GetAllGroups().FirstOrDefaultAsync(g => g.Name == command.Name);
        if (group != null)
        {
            return false;
        }
        return true;
    }
}
