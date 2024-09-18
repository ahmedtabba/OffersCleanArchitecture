using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.NotificationGroup;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.NotificationGroups.Commands.CreateNotificationGroup;
using Offers.CleanArchitecture.Application.NotificationGroups.Commands.DeleteNotificationGroup;
using Offers.CleanArchitecture.Application.NotificationGroups.Commands.UpdateNotificationGroup;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroup;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroupsWithPagination;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationObjectTypes;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotifications;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroup;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroupsWithPagination;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetRoles;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Controllers;

[ApiController]
public class NotificationGroupsController : ControllerBase
{
    private readonly ILogger<NotificationGroupsController> _logger;
    private readonly ISender _sender;

    public NotificationGroupsController(ILogger<NotificationGroupsController> logger,
                                        ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }
    [Route(ApiRoutes.NotificationGroup.Create)]
    [HttpPost]
    [Permission(RoleConsistent.NotificationGroup.Add)]
    public async Task<IActionResult> CreateNotificationGroup([FromBody] CreateNotificationGroupCommand command)
    {
        try
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.NotificationGroup.Update)]
    [HttpPut]
    [Permission(RoleConsistent.NotificationGroup.Edit)]
    public async Task<IActionResult> UpdateNotificationGroup([FromBody] UpdateNotificationGroupDto dto, [FromRoute] Guid notificationGroupId)
    {
        try
        {
            UpdateNotificationGroupCommand command = new UpdateNotificationGroupCommand
            {
                Id = notificationGroupId,
                Name = dto.Name,
                NotificationsIds = dto.NotificationsIds,
            };
            await _sender.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.NotificationGroup.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.NotificationGroup.Delete)]
    public async Task<IActionResult> DeleteNotificationGroup([FromRoute] DeleteNotificationGroupCommand command)
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
    [HttpGet(ApiRoutes.NotificationGroup.Get)]
    [Permission(RoleConsistent.NotificationGroup.Browse)]
    public async Task<IActionResult> Get([FromRoute] GetNotificationGroupQuery query)
    {
        try
        {
            var result = await _sender.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages }); ;
        }

    }

    [HttpGet(ApiRoutes.NotificationGroup.GetAll)]
    [Permission(RoleConsistent.NotificationGroup.Browse)]
    public async Task<IActionResult> GetAll([FromQuery] GetNotificationGroupsWithPaginationQuery query)
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

    [HttpGet(ApiRoutes.Notification.GetAll)]
    [Permission(RoleConsistent.NotificationGroup.BrowseNotifications)]
    public async Task<IActionResult> GetAllNotifications()
    {
        try
        {
            GetNotificationsQuery getNotificationssQuery = new GetNotificationsQuery();
            var result = await _sender.Send(getNotificationssQuery);

            return Ok(result.ToList().BuildTree(new List<GetNotificationsDto>()));
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [HttpGet(ApiRoutes.Notification.GetNotificationObjectTypes)]
    [Authorize]
    public async Task<IActionResult> GetNotificationObjectTypes()
    {
        try
        {
            GetNotificationObjectTypesQuery query = new GetNotificationObjectTypesQuery();
            var result = await _sender.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }
}
