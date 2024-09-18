using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroup;
public class GetGroupQueryValidator : AbstractValidator<GetGroupQuery>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<GetGroupQueryValidator> _logger;

    public GetGroupQueryValidator(IApplicationGroupManager applicationGroupManager, ILogger<GetGroupQueryValidator> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
        RuleFor(g => g.groupId)
        .NotEmpty().WithMessage("GroupId must passed")
        .CustomAsync(async (name, context, cancellationToken) =>
        {
            if (!await IsGroupExisted(context.InstanceToValidate))
            {
                context.AddFailure("Get Group", "Group is not found");
            }
        });

    }

    public async Task<bool> IsGroupExisted(GetGroupQuery command)
    {
        var group = await _applicationGroupManager.FindByIdAsync(command.groupId);
        if (group is null)
            return false;
        return true;
    }
}
