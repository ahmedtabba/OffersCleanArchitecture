using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;
using Offers.CleanArchitecture.Application.UserGroups.Commands.UpdateGroup;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroup;
public class GetGroupQuery : IRequest<IApplicationGroup>
{
    public string groupId { get; set; } = null!;
}

public class GetGroupQueryHandler : IRequestHandler<GetGroupQuery, IApplicationGroup>
{
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly ILogger<GetGroupQueryHandler> _logger;

    public GetGroupQueryHandler(IApplicationGroupManager applicationGroupManager, ILogger<GetGroupQueryHandler> logger)
    {
        _applicationGroupManager = applicationGroupManager;
        _logger = logger;
    }
    public async Task<IApplicationGroup> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await _applicationGroupManager.FindByIdAsync(request.groupId);
        //Guard.Against.NotFound(request.groupId, group);

        // Get Users of this group and fill them in helper collection
        await new GroupMethodsHelper(_applicationGroupManager).FillApplicationUsersHelper(group.Id, group);

        // Get Roles of this group and fill them in helper collection
        await new GroupMethodsHelper(_applicationGroupManager).FillApplicationRolesHelper(group.Id, group);

        // Here a cycle fault occurs if we return group, so we return Dto Model
        ApplicationGroupDto groupDto = new ApplicationGroupDto
        {
            Id = group.Id,
            Description = group.Description,
            Name = group.Name,
            ApplicationRolesHelper = group.ApplicationRolesHelper,
            ApplicationUsersHelper = group.ApplicationUsersHelper,
        };

        return groupDto;
    }

}
