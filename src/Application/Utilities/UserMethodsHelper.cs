using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;

namespace Offers.CleanArchitecture.Application.Utilities;
public class UserMethodsHelper
{
    private readonly IApplicationGroupManager _applicationGroupManager;

    public UserMethodsHelper(IApplicationGroupManager applicationGroupManager)
    {
        _applicationGroupManager = applicationGroupManager;
    }

    public async Task FillApplicationGroupHelper(string userId, IApplicationUser user)
    {
        var groups = await _applicationGroupManager.GetUserGroupsAsync(userId);
        foreach (var group in groups)
        {
            var applicationGroupHelper = new ApplicationGroupHelper
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
            };
            user.UserGroups.Add(applicationGroupHelper);
        }
    }
}
