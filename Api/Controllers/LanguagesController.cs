using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.Language;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Languages.Commands.CreateLanguage;
using Offers.CleanArchitecture.Application.Languages.Commands.DeleteLanguage;
using Offers.CleanArchitecture.Application.Languages.Commands.UpdateLanguage;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageQuery;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithGlossariesWithPagination;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithPagination;
using Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageWithGlossaries;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Controllers;
[ApiController]
public class LanguagesController : ControllerBase
{
    private readonly ILogger<LanguagesController> _logger;
    private readonly ISender _sender;

    public LanguagesController(ILogger<LanguagesController> logger,
                               ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [Route(ApiRoutes.Language.Create)]
    [HttpPost]
    [Permission(RoleConsistent.Language.Add)]
    public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageCommand command)
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

    [Route(ApiRoutes.Language.GetAll)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetLanguagesWithPagination([FromQuery] GetLanguagesWithPaginationQuary query)
    {
        try
        {
            return Ok(await _sender.Send(query));
        }
        catch (ValidationException ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Language.GetAllWithGlossaries)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetLanguagesWithGlossaries([FromQuery] GetLanguagesWithGlossariesWithPaginationQuery query)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            return Ok(JsonSerializer.Deserialize<dynamic>(await _sender.Send(query), options));
        }
        catch (ValidationException ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Language.GetLanguageWithGlossaries)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetLanguageWithGlossaries([FromRoute] GetLanguageWithGlossariesQuery query)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            return Ok(JsonSerializer.Deserialize<dynamic>(await _sender.Send(query), options));
        }
        catch (ValidationException ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Language.Get)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetLanguage([FromRoute] GetLanguageQuery query)
    {
        try
        {
            return Ok(await _sender.Send(query));
        }
        catch (ValidationException ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Language.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.Language.Delete)]
    public async Task<IActionResult> DeleteLanguage([FromRoute] DeleteLanguageCommand command)
    {
        try
        {
            await _sender.Send(command);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }


    [Route(ApiRoutes.Language.Update)]
    [HttpPut]
    [Permission(RoleConsistent.Language.Edit)]
    public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageCommandDto dto, Guid languageId)
    {
        try
        {
            UpdateLanguageCommand command = new UpdateLanguageCommand
            {
                Id = languageId,
                Name = dto.Name,
                Code = dto.Code,
                RTL = dto.RTL,
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
}
