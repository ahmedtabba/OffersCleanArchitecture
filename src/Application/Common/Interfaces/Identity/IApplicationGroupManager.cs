using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
public interface IApplicationGroupManager : IDisposable
{
    //public IQueryable<IApplicationGroup> Groups { get; }
    //public Task<Result> CreateGroupAsync(IApplicationGroup group);
    public Task<string> CreateGroupAsync(CreateGroupRequest group);
    //public Result CreateGroup(IApplicationGroup group);
    public string CreateGroup(CreateGroupRequest group);
    public Result SetGroupRoles(string groupId, params string[] roleNames);
    public Task<Result> SetGroupRolesAsync(string groupId, params string[] roleNames);
    public Result SetGroupRolesByRolesIds(string groupId, params string[] roleIds);
    public Task<Result> SetGroupRolesByRolesIdsAsync(string groupId, params string[] roleIds);
    public Task<Result> SetUserGroupsAsync(string userId, params string[] groupIds);
    public Task<Result> SetUserGroups(string userId, params string[] groupIds);
    public Task<Result> RefreshUserGroupRoles(string userId);
    public Task<Result> RefreshUserGroupRolesAsync(string userId);
    public Task<Result> DeleteGroupAsync(string groupId);
    public Result DeleteGroup(string groupId);
    //public Task<Result> UpdateGroupAsync(IApplicationGroup group);
    public Task<Result> UpdateGroupAsync(UpdateGroupRequest group);
    //public Task<Result> UpdateGroup(IApplicationGroup group);
    public Result UpdateGroup(UpdateGroupRequest group);
    public Task<IEnumerable<IApplicationGroup>> GetUserGroupsAsync(string userId);
    public IEnumerable<IApplicationGroup> GetUserGroups(string userId);
    public Task<IEnumerable<IApplicationRole>> GetGroupRolesAsync(string groupId);
    public IEnumerable<IApplicationRole> GetGroupRoles(string groupId);
    public IEnumerable<IApplicationUser> GetGroupUsers(string groupId);
    public Task<IEnumerable<IApplicationUser>> GetGroupUsersAsync(string groupId);
    //public IEnumerable<IApplicationGroupRole> GetUserGroupRoles(string userId);
    public IEnumerable<string> GetUserGroupRoles(string userId);
    // public Task<IEnumerable<IApplicationGroupRole>> GetUserGroupRolesAsync(string userId);
    public Task<IEnumerable<string>> GetUserGroupRolesAsync(string userId);
    public Task<IApplicationGroup> FindByIdAsync(string id);
    public IApplicationGroup FindById(string id);
    public IQueryable<IApplicationGroup> GetAllGroups();
    public Task<IEnumerable<IApplicationRole>> GetAllRoles();




}
