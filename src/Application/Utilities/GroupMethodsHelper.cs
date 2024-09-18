using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;

namespace Offers.CleanArchitecture.Application.Utilities;
public class GroupMethodsHelper
{
    private readonly IApplicationGroupManager _applicationGroupManager;

    public GroupMethodsHelper(IApplicationGroupManager applicationGroupManager)
    {
        _applicationGroupManager = applicationGroupManager;
    }
    public async Task FillApplicationUsersHelper(string groupId, IApplicationGroup group)
    {
        var users = await _applicationGroupManager.GetGroupUsersAsync(groupId);
        foreach (var user in users)
        {
            var applicationUserHelper = new ApplicationUserHelper
            {
                Id = user.Id,
                UserName = user.UserName,
            };
            group.ApplicationUsersHelper.Add(applicationUserHelper);
        }
    }

    public async Task FillApplicationRolesHelper(string groupId, IApplicationGroup group)
    {
        var roles = await _applicationGroupManager.GetGroupRolesAsync(groupId);
        foreach (var role in roles)
        {
            var applicationRoleHelper = new ApplicationRoleHelper
            {
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
                ConcurrencyStamp = role.ConcurrencyStamp,
            };
            group.ApplicationRolesHelper.Add(applicationRoleHelper);
        }
    }
}
