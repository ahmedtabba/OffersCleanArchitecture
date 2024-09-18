using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.UserGroup;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.UserGroups.Commands.CreateGroup;
using Offers.CleanArchitecture.Application.UserGroups.Commands.DeleteGroup;
using Offers.CleanArchitecture.Application.UserGroups.Commands.UpdateGroup;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroup;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroupsWithPagination;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetRoles;
using Offers.CleanArchitecture.Infrastructure.Identity;
using Offers.CleanArchitecture.Infrastructure.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Offers.CleanArchitecture.Api.Controllers;
[ApiController]
public class UserGroupController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly ILogger<UserGroupController> _logger;

    public UserGroupController(ISender sender,
                              IMapper mapper,
                              ILogger<UserGroupController> logger)
    {
        _sender = sender;
        _mapper = mapper;
        _logger = logger;
    }
    [HttpPost(ApiRoutes.UserGroup.Create)]
    [Permission(RoleConsistent.UserGroup.Add)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command)
    {
        try
        {
            return Ok(await _sender.Send(command));
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [HttpPut(ApiRoutes.UserGroup.Update)]
    [Permission(RoleConsistent.UserGroup.Edit)]
    public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupCommandDto commandDto,[FromRoute]string groupId)
    {
        try
        {
            UpdateGroupCommand command = new UpdateGroupCommand
            {
                GroupId = groupId,
                Name = commandDto.Name,
                Description = commandDto.Description,
                Roles = commandDto.Roles
            };
            await _sender.Send(command);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        return NoContent();
    }

    [HttpGet(ApiRoutes.UserGroup.Get)]
    [Permission(RoleConsistent.UserGroup.BrowseUserGroups)]
    public async Task<IActionResult> Get([FromRoute] GetGroupQuery query)
    {
        try
        {
            var result = await _sender.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        
    }

    [HttpGet(ApiRoutes.UserGroup.GetAll)]
    [Permission(RoleConsistent.UserGroup.BrowseUserGroups)]
    public async Task<IActionResult> GetAll([FromQuery] GetGroupsWithPaginationQuery query)
    {
        try
        {
            var result = await _sender.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }

    }

    [HttpDelete(ApiRoutes.UserGroup.Delete)]
    [Permission(RoleConsistent.UserGroup.Delete)]
    public async Task<IActionResult> Delete([FromRoute] DeleteGroupCommand command)
    {
        try
        {
            await _sender.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [HttpGet(ApiRoutes.Role.GetAll)]
    [Permission(RoleConsistent.UserGroup.BrowseRoles)]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            GetRolesQuery getRolesQuery = new GetRolesQuery();
            var result = await _sender.Send(getRolesQuery);

            return Ok(result.ToList().BuildTree(new List<IApplicationRole>()));
        }
        catch (Exception ex) 
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

}
