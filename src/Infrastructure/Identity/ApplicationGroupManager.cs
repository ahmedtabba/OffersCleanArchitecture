using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataCollector.CleanArchitecture.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Domain.Constants;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class ApplicationGroupManager : IApplicationGroupManager
{
    private readonly CleanArchitectureIdentityDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    //private ApplicationGroupStore _groupStore;

    public ApplicationGroupManager(CleanArchitectureIdentityDbContext dbContext,
                                   UserManager<ApplicationUser> userManager,
                                   RoleManager<ApplicationRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
       // _groupStore = new ApplicationGroupStore(DbContext);
    }


  //public IQueryable<IApplicationGroup> Groups { get { return _groupStore.Groups; } }// they dosn't containe ApplicationUser or ApplicationRole

    public string CreateGroup(CreateGroupRequest group)
    {
        var applicationGroupToAdd = new ApplicationGroup
        {
            Name = group.Name,
            Description = group.Description,
        };
        _dbContext.ApplicationGroups.Add(applicationGroupToAdd);
        _dbContext.SaveChanges();
        return applicationGroupToAdd.Id;
    }

    public async Task<string> CreateGroupAsync(CreateGroupRequest group)
    {
        var applicationGroupToAdd = new ApplicationGroup
        {
            Name = group.Name,
            Description = group.Description,
        };
        await _dbContext.ApplicationGroups.AddAsync(applicationGroupToAdd);
        await _dbContext.SaveChangesAsync();
        //await _groupStore.CreateAsync(applicationGroupToAdd);
        return applicationGroupToAdd.Id;
    }

    public Result DeleteGroup(string groupId)
    {
        var group = _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .Include(g => g.ApplicationRoles)
            .FirstOrDefault(g => g.Id == groupId);
        if (!group!.ApplicationRoles.Any()) // group don't have roles
        {
            if (!group!.ApplicationUsers.Any()) //group don't have roles or users
            {
                _dbContext.ApplicationGroups.Remove(group);
                _dbContext.SaveChanges();
            }
            else // group don't have roles but has users
            {
                group.ApplicationUsers.Clear();
                _dbContext.ApplicationGroups.Remove(group);
                _dbContext.SaveChanges();
            }
        }
        else // group has roles
        {
            if (!group!.ApplicationUsers.Any()) // group has roles but don't have users
            {
                group.ApplicationRoles.Clear();
                _dbContext.ApplicationGroups.Remove(group);
                _dbContext.SaveChanges();
            }
            else // group has roles and users
            {
                var currentGroupMembers = group.ApplicationUsers;

                // remove the roles from the group:
                group.ApplicationRoles.Clear();

                // Remove all the users:
                group.ApplicationUsers.Clear();

                _dbContext.ApplicationGroups.Remove(group);
                _dbContext.SaveChanges();

                // Reset all the user roles:
                foreach (var user in currentGroupMembers)
                {
                    RefreshUserGroupRoles(user.Id);
                }
            }
        }
        return IdentityResult.Success.ToApplicationResult();
    }

    public async Task<Result> DeleteGroupAsync(string groupId)
    {
        var group = await _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .Include(g => g.ApplicationRoles)
            .FirstOrDefaultAsync(g => g.Id == groupId);
        if (!(group!.ApplicationRoles.Count>0)) // group don't have roles
        {
            if (!(group!.ApplicationUsers.Count > 0)) //group don't have roles or users
            {
                _dbContext.ApplicationGroups.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
            else // group don't have roles but has users
            {
                group.ApplicationUsers.Clear();
                _dbContext.ApplicationGroups.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
        }
        else // group has roles
        {
            if (!(group!.ApplicationUsers.Count > 0)) // group has roles but don't have users
            {
                group.ApplicationRoles.Clear();
                _dbContext.ApplicationGroups.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
            else // group has roles and users
            {
                var currentGroupMembers = group.ApplicationUsers;

                // remove the roles from the group:
                group.ApplicationRoles.Clear();

                // Remove all the users:
                group.ApplicationUsers.Clear();

                _dbContext.ApplicationGroups.Remove(group);
                await _dbContext.SaveChangesAsync();

                // Reset all the user roles:
                foreach (var user in currentGroupMembers)
                {
                    await RefreshUserGroupRolesAsync(user.Id);
                }
            }
        }
        return IdentityResult.Success.ToApplicationResult();

    }


    public IApplicationGroup FindById(string id)
    {
        return _dbContext.ApplicationGroups.Find(id)!;
    }

    public async Task<IApplicationGroup> FindByIdAsync(string id)
    {
        return await _dbContext.ApplicationGroups.FindAsync(id);
    }

    public IEnumerable<IApplicationRole> GetGroupRoles(string groupId)
    {
        var group = _dbContext.ApplicationGroups
            .Include(g => g.ApplicationRoles)
            .FirstOrDefault(g => g.Id == groupId);
        return group.ApplicationRoles;
    }

    public async Task<IEnumerable<IApplicationRole>> GetGroupRolesAsync(string groupId)
    {
        var group = await _dbContext.ApplicationGroups
            .Include(g => g.ApplicationRoles)
            .FirstOrDefaultAsync(g => g.Id == groupId);
        return group.ApplicationRoles;
    }

    public IEnumerable<IApplicationUser> GetGroupUsers(string groupId)
    {
        var group = _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .FirstOrDefault(g => g.Id == groupId);
        return group.ApplicationUsers;
    }

    public async Task<IEnumerable<IApplicationUser>> GetGroupUsersAsync(string groupId)
    {
        var group = await _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .FirstOrDefaultAsync(g => g.Id == groupId);
        return group.ApplicationUsers;
    }

    public IEnumerable<string> GetUserGroupRoles(string userId)
    {
        var userGroups = GetUserGroups(userId);
        var userGroupRoles = new List<ApplicationRole>();
        var userGroupRolesIds = new List<string>();
        foreach (var group in userGroups)
        {
            var x = _dbContext.ApplicationGroups
                .Include(g => g.ApplicationRoles)
                .FirstOrDefault(g => g.Id == group.Id);
            userGroupRoles.AddRange(x.ApplicationRoles);
        }
        foreach (var applicationRole in userGroupRoles)
        {
            userGroupRolesIds.Add(applicationRole.Id);
        }
        return userGroupRolesIds;
    }

    public async Task<IEnumerable<string>> GetUserGroupRolesAsync(string userId)
    {
        var userGroups = await this.GetUserGroupsAsync(userId);
        var userGroupRoles = new List<ApplicationRole>();
        var userGroupRolesIds = new List<string>();
        foreach (var group in userGroups)
        {
            var x = await _dbContext.ApplicationGroups
                .Include(g => g.ApplicationRoles)
                .FirstOrDefaultAsync(g => g.Id == group.Id);
            userGroupRoles.AddRange(x.ApplicationRoles);
        }
        foreach (var applicationRole in userGroupRoles)
        {
            userGroupRolesIds.Add(applicationRole.Id);
        }
        return userGroupRolesIds;
    }

    public IEnumerable<IApplicationGroup> GetUserGroups(string userId)
    {
        return  _dbContext.ApplicationGroups
                      .Where(g => g.ApplicationUsers.Any(u => u.Id == userId)).ToList();
    }

    public async Task<IEnumerable<IApplicationGroup>> GetUserGroupsAsync(string userId)
    {
        return await _dbContext.ApplicationGroups
                      //.Include(g=> g.ApplicationUsers)
                      //.Include(g=>g.ApplicationRoles)
                      .Where(g=>g.ApplicationUsers.Any(u=>u.Id == userId)).ToListAsync();
    }

    public async Task<Result> RefreshUserGroupRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentNullException("User");
        }
        // Remove user from previous roles:
        var oldUserRoles = await _userManager.GetRolesAsync(user);
        if (oldUserRoles.Count > 0)
        {
            await _userManager.RemoveFromRolesAsync(user, oldUserRoles.ToArray());
        }

        // Find the roles this user is entitled to from group membership:
        var newGroupRoles =  GetUserGroupRoles(userId);

        // Get the role names:
        var allRoles = await _roleManager.Roles.ToListAsync();
        // Get the role names accodring to UserGroup Roles :
        var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(x => x == r.Id));
        var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

        // Add the user to the proper roles
        await _userManager.AddToRolesAsync(user, roleNames);

        return IdentityResult.Success.ToApplicationResult();
    }

    public async Task<Result> RefreshUserGroupRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentNullException("User");
        }
        // Remove user from previous roles:
        var oldUserRoles = await _userManager.GetRolesAsync(user);
        if (oldUserRoles.Count > 0)
        {
            await _userManager.RemoveFromRolesAsync(user, oldUserRoles.ToArray());
        }

        // Find the roles this user is entitled to from group membership:
        var newGroupRoles = await this.GetUserGroupRolesAsync(userId);

        // Get the role names:
        var allRoles = await _roleManager.Roles.ToListAsync();
        // Get the role names accodring to UserGroup Roles :
        var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(x=> x == r.Id));
        var roleNames = addTheseRoles.Select(n => n.Name).ToArray();

        // Add the user to the proper roles
        await _userManager.AddToRolesAsync(user, roleNames);

        return IdentityResult.Success.ToApplicationResult();//TODO: Result للمراجعة مع ابو خالد من اجل جميع التوابع التي تعيد
    }

    public Result SetGroupRoles(string groupId, params string[] roleNames)
    {
        var group = _dbContext.ApplicationGroups
              .Include(x => x.ApplicationUsers)
              .Include(x => x.ApplicationRoles)
              .FirstOrDefault(x => x.Id == groupId);
        Guard.Against.NotFound(groupId, group);
        // Clear all the roles associated with this group:
        group.ApplicationRoles.Clear();
        _dbContext.SaveChanges();

        var newRoles = _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name)).ToList();

        // Add the new roles passed in:
        foreach (var role in newRoles)
        {
            group.ApplicationRoles.Add(role);
        }
        _dbContext.SaveChanges();

        // Reset the roles for all affected users:
        foreach (var user in group.ApplicationUsers)
        {
            this.RefreshUserGroupRoles(user.Id).Wait();
        }

        return (IdentityResult.Success).ToApplicationResult();
    }

    public async Task<Result> SetGroupRolesAsync(string groupId, params string[] roleNames)
    {
        var group = await _dbContext.ApplicationGroups
            .Include(x => x.ApplicationUsers)
            .Include(x => x.ApplicationRoles)
            .FirstOrDefaultAsync(x => x.Id == groupId);
        Guard.Against.NotFound(groupId, group);
        // Clear all the roles associated with this group:
        group.ApplicationRoles.Clear();
        await _dbContext.SaveChangesAsync();

        var newRoles = await _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name)).ToListAsync();

        // Add the new roles passed in:
        foreach (var role in newRoles)
        {
            group.ApplicationRoles.Add(role);
        }
        await _dbContext.SaveChangesAsync();

        // Reset the roles for all affected users:
        foreach (var user in group.ApplicationUsers)
        {
            await RefreshUserGroupRolesAsync(user.Id);
        }

        return (IdentityResult.Success).ToApplicationResult();
    }

    public Result SetGroupRolesByRolesIds(string groupId, params string[] roleIds)
    {
        var group = _dbContext.ApplicationGroups
             .Include(x => x.ApplicationUsers)
             .Include(x => x.ApplicationRoles)
             .FirstOrDefault(x => x.Id == groupId);
        Guard.Against.NotFound(groupId, group);
        // Clear all the roles associated with this group:
        group.ApplicationRoles.Clear();
        _dbContext.SaveChanges();

        var newRoles = _roleManager.Roles.Where(r => roleIds.Any(n => n == r.Id)).ToList();

        // Add the new roles passed in:
        foreach (var role in newRoles)
        {
            group.ApplicationRoles.Add(role);
        }
        _dbContext.SaveChanges();

        // Reset the roles for all affected users:
        foreach (var user in group.ApplicationUsers)
        {
            this.RefreshUserGroupRoles(user.Id).Wait();
        }

        return (IdentityResult.Success).ToApplicationResult();
    }

    public async Task<Result> SetGroupRolesByRolesIdsAsync(string groupId, params string[] roleIds)
    {
        var group = await _dbContext.ApplicationGroups
            .Include(x => x.ApplicationUsers)
            .Include(x => x.ApplicationRoles)
            .FirstOrDefaultAsync(x => x.Id == groupId);
        Guard.Against.NotFound(groupId, group);
        // Clear all the roles associated with this group:
        group.ApplicationRoles.Clear();
        await _dbContext.SaveChangesAsync();

        var newRoles = await _roleManager.Roles.Where(r => roleIds.Any(n => n == r.Id)).ToListAsync();

        // Add the new roles passed in:
        foreach (var role in newRoles)
        {
            group.ApplicationRoles.Add(role);
        }
        await _dbContext.SaveChangesAsync();

        // Reset the roles for all affected users:
        foreach (var user in group.ApplicationUsers)
        {
            await this.RefreshUserGroupRolesAsync(user.Id);
        }

        return (IdentityResult.Success).ToApplicationResult();

        /*
        // Clear all the roles associated with this group:
        //var thisGroup = await this.Groups.Include(x => x.ApplicationRoles).Where(x => x.Id == groupId).FirstOrDefaultAsync();
        var q = await this.Groups
            .Include(x=>x.)
            .Where(x => x.Id == groupId).FirstOrDefaultAsync();
        thisGroup.ApplicationRoles.Clear();
        await _db.SaveChangesAsync();

        // Add the new roles passed in:
        var newRoles = _roleManager.Roles.Where(r => roleIds.Any(n => n == r.Id));
        foreach (var role in newRoles)
        {
            thisGroup.ApplicationRoles.Add(new ApplicationGroupRole { ApplicationGroupId = groupId, ApplicationRoleId = role.Id });
        }
        await _db.SaveChangesAsync();

        // Reset the roles for all affected users:
        foreach (var groupUser in thisGroup.ApplicationUsers)
        {
            await this.RefreshUserGroupRolesAsync(groupUser.ApplicationUserId);
        }*/

    }

    public async Task<Result> SetUserGroups(string userId, params string[] groupIds)
    {
        // Clear current group membership:
        var user = await  _userManager.FindByIdAsync(userId);
        var currentGroups = _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .Where(g => g.ApplicationUsers.Any(u => u.Id == userId)).ToList();
        foreach (var group in currentGroups)
        {
            group.ApplicationUsers.Remove(user);
        }
        _dbContext.SaveChanges();

        // Add the user to the new groups:
        foreach (string groupId in groupIds)
        {
            var newGroup = _dbContext.ApplicationGroups
                .FirstOrDefault(g => g.Id == groupId);
            newGroup!.ApplicationUsers.Add(user!);
        }
        _dbContext.SaveChanges();

        await RefreshUserGroupRoles(userId);
        return (IdentityResult.Success).ToApplicationResult(); ;
    }

    public async Task<Result> SetUserGroupsAsync(string userId, params string[] groupIds)
    {
        // Clear current group membership:
        var user = await _userManager.FindByIdAsync(userId);
        var currentGroups = await _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .Where(g => g.ApplicationUsers.Any(u => u.Id == userId)).ToListAsync();
        foreach (var group in currentGroups)
        {
            group.ApplicationUsers.Remove(user!);
        }
        await _dbContext.SaveChangesAsync();

        // Add the user to the new groups:
        foreach (string groupId in groupIds)
        {
            var newGroup = await _dbContext.ApplicationGroups
                .FirstOrDefaultAsync(g => g.Id == groupId);
            newGroup!.ApplicationUsers.Add(user!);
        }
        await _dbContext.SaveChangesAsync();

        await RefreshUserGroupRolesAsync(userId);
        return (IdentityResult.Success).ToApplicationResult();
    }

    public Result UpdateGroup(UpdateGroupRequest groupRequest)
    {
        // get group
        var group = _dbContext.ApplicationGroups
            .Include(g => g.ApplicationUsers)
            .FirstOrDefault(x => x.Id == groupRequest.GroupId);
        // update group name and description
        group.Name = groupRequest.Name;
        group.Description = groupRequest.Description;
        _dbContext.Entry<ApplicationGroup>(group).State = EntityState.Modified;
        _dbContext.SaveChanges();
        // update group roles and user roles
        var result = SetGroupRolesByRolesIds(groupRequest.GroupId, groupRequest.Roles.ToArray());
        return result;
    }

    public async Task<Result> UpdateGroupAsync(UpdateGroupRequest groupRequest)
    {
        // get group
        var group = await _dbContext.ApplicationGroups
            .Include(g=>g.ApplicationUsers)
            .FirstOrDefaultAsync(x => x.Id == groupRequest.GroupId);
        // update group name and description
        group.Name = groupRequest.Name;
        group.Description = groupRequest.Description;
        _dbContext.Entry<ApplicationGroup>(group).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        // update group roles and user roles
        var result = await SetGroupRolesByRolesIdsAsync(groupRequest.GroupId, groupRequest.Roles.ToArray());
        return result;
    }

    public IQueryable<IApplicationGroup> GetAllGroups()
    {
        return _dbContext.Set<ApplicationGroup>().AsNoTracking().AsQueryable();
    }

    public async Task<IEnumerable<IApplicationRole>> GetAllRoles()
    {
        var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        return roles;
    }



    //Dispose section

    private bool disposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}
