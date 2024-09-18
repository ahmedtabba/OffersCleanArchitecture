using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.UserGroups.Queries.GetGroupsWithPagination;
using Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAllAsReadNotification;
using Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsReadNotification;
using Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsUnReadNotification;
using Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
using Offers.CleanArchitecture.Infrastructure.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Offers.CleanArchitecture.Api.Controllers;
[ApiController]
public class UserNotificationsController : ControllerBase
{
    private readonly ILogger<UserNotificationsController> _logger;
    private readonly ISender _sender;

    public UserNotificationsController(ILogger<UserNotificationsController> logger,
                                       ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpGet(ApiRoutes.UserNotification.GetAll)]
    [Permission(RoleConsistent.UserNotification.Browse)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUserNotificationsQuery query)
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

    [HttpPut(ApiRoutes.UserNotification.MakeAsRead)]
    [Permission(RoleConsistent.UserNotification.MakeAsRead)]
    public async Task<IActionResult> MakeAsReadNotification([FromRoute] MakeAsReadNotificationCommand command)
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

    [HttpPut(ApiRoutes.UserNotification.MakeAsUnRead)]
    [Permission(RoleConsistent.UserNotification.MakeAsUnRead)]
    public async Task<IActionResult> MakeAsUnReadNotification([FromRoute] MakeAsUnReadNotificationCommand command)
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

    [HttpPut(ApiRoutes.UserNotification.MakeAllAsRead)]
    [Permission(RoleConsistent.UserNotification.MakeAsRead)]
    public async Task<IActionResult> MakeAllAsReadNotification([FromRoute] MakeAllAsReadNotificationCommand command)
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
}
