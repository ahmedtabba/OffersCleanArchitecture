using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Identity.Commands.Login;
using Offers.CleanArchitecture.Application.Identity.Commands.Register;
using Offers.CleanArchitecture.Application.Identity.Queries.GetUsersWithPagination;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.NeededDto.Identity;
using Offers.CleanArchitecture.Application.Identity.Commands.CreateUser;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Identity.Queries.GetUser;
using Offers.CleanArchitecture.Application.Identity.Commands.DeleteUser;
using Offers.CleanArchitecture.Application.Identity.Commands.UpdateUserCommand;
using Offers.CleanArchitecture.Application.Identity.Commands.ResetMyPasswordCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Authorization;
using Offers.CleanArchitecture.Application.Identity.Commands.ResetPasswordByAdminCommand;
using Polly;
using Offers.CleanArchitecture.Api.PollyHandling;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Infrastructure.Utilities;
using Offers.CleanArchitecture.Application.Identity.Commands.SignUp;
using Offers.CleanArchitecture.Application.Identity.Queries.GetJobRoles;

namespace Offers.CleanArchitecture.Api.Controllers;

[ApiController]
public class IdentityController : ControllerBase
{
    readonly private ISender _sender;
    private readonly IUser _user;
    private readonly ILogger<IdentityController> _logger;
    private readonly PollyPolicy _pollyPolicy;

    //private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;

    public IdentityController(ISender sender,
                              IUser user,
                              ILogger<IdentityController> logger,
                              //[FromKeyedServices("retry-pipe")] ResiliencePipeline<HttpResponseMessage> pipeline
                              PollyPolicy pollyPolicy
                              )
    {
        _sender = sender;
        _user = user;
        _logger = logger;
        _pollyPolicy = pollyPolicy;
        //_pipeline = pipeline;
    }
    [AllowAnonymous]
    [Route(ApiRoutes.Identity.Login)]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        /*
        var response = await _pollyPolicy.RetryObjectResultFife.ExecuteAsync(
           async () =>
           {
               try
               {
                   var result = await _sender.Send(command);
                   return Ok(result);
               }
               catch (Exception ex)
               {

                   return BadRequest(ex.Message);
               }
           });
        return response;
        */
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

    [Route(ApiRoutes.Identity.Register)]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)// Untested, we don't use it now, but if it be used, we should test it
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

    [Route(ApiRoutes.Identity.SignUp)]
    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
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

    [Route(ApiRoutes.Identity.Create)]
    [HttpPost]
    [Permission(RoleConsistent.Identity.Add)]
    public async Task<IActionResult> CreateUserByAdmin([FromForm] CreateUserDto dto)
    {
        try
        {
            CreateUserCommand command = new CreateUserCommand
            {
                Email = dto.Email,
                Password = dto.Password,
                FullName = dto.FullName,
                UserName = dto.UserName,
                CountryId = dto.CountryId,
                LanguageId = dto.LanguageId,
                PhoneNumber = dto.PhoneNumber,
                JobRole = dto.JobRole.ToJobRole(),
                GroupIds = dto.GroupIds,
                NotificationGroupIds = dto.NotificationGroupIds.ToStringList(),
                File = dto.File != null? dto.File.ToFileDto() : null,
            };
            var result = await _sender.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest (new { Errors = messages } );
        }
    }

    [Route(ApiRoutes.Identity.GetAll)]
    [HttpGet]
    [Permission(RoleConsistent.Identity.BrowseUsers)]
    public async Task<IActionResult> GetUsersWithPagination([FromQuery] GetUsersWithPaginationQuery query)
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

    [Route(ApiRoutes.Identity.Get)]
    [HttpGet]
    [Permission(RoleConsistent.Identity.BrowseUsers)]
    public async Task<IActionResult> GetUser([FromRoute] GetUserQuery query)
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

    [Route(ApiRoutes.Identity.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.Identity.Delete)]
    public async Task<IActionResult> DeleteUser([FromRoute] DeleteUserCommand command)
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

    [Route(ApiRoutes.Identity.Update)]
    [HttpPut]
    [Permission(RoleConsistent.Identity.Edit)]
    public async Task<IActionResult> UpdateUser([FromRoute] string userId,[FromForm]UpdateUserByAdminDto dto)
    {
        UpdateUserCommand command = new UpdateUserCommand
        {
            FullName = dto.FullName,
            UserId = userId,
            PhoneNumber = dto.PhoneNumber,
            CountryId = dto.CountryId,
            LanguageId = dto.LanguageId,
            JobRole = dto.JobRole.ToJobRole(),
            GroupIds = dto.GroupIds,
            NotificationGroupIds = dto.NotificationGroupIds.ToStringList(),
            File = dto.File != null ? dto.File.ToFileDto() : null,
        };
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

    [Route(ApiRoutes.Identity.ResetMyPassword)]
    [HttpPost]
    [Permission(RoleConsistent.Identity.ResetMyPassword)]
    public async Task<IActionResult> ResetMyPassword([FromBody] ResetMyPasswordDto dto)
    {
        try
        {
            ResetMyPasswordCommand command = new ResetMyPasswordCommand
            {
                UserId = _user.Id,
                Password = dto.Password,
                PasswordConfirmation = dto.PasswordConfirmation,
            };
            var token = await _sender.Send(command);
            return Ok(token);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Identity.ResetPassword)]
    [HttpPost]
    [Permission(RoleConsistent.Identity.ResetPasswordByAdmin)]
    public async Task<IActionResult> ResetPasswordByAdmin([FromBody] ResetPasswordByAdminDto dto ,[FromRoute] string userId)
    {
        try
        {
            ResetPasswordByAdminCommand command = new ResetPasswordByAdminCommand
            {
                UserId = userId,
                Password = dto.NewPssword,
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

    [Route(ApiRoutes.Identity.GetJobRoles)]
    [HttpGet]
    public async Task<IActionResult> GetJobRoles()
    {
        try
        {
            GetJobRolesQuery query = new GetJobRolesQuery();
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
