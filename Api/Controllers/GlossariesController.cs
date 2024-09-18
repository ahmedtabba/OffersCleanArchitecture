using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.Glossary;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Glossaries.Commands.UpdateGlossary;
using Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossariesForAdminWithPagination;
using Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossary;
using Offers.CleanArchitecture.Infrastructure.Utilities;


namespace Offers.CleanArchitecture.Api.Controllers;
[ApiController]
public class GlossariesController : ControllerBase
{
    private readonly ILogger<GlossariesController> _logger;
    private readonly ISender _sender;

    public GlossariesController(ILogger<GlossariesController> logger,
                                ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [Route(ApiRoutes.Glossary.Update)]
    [HttpPut]
    [Permission(RoleConsistent.Glossary.Edit)]
    public async Task<IActionResult> UpdateGlossary([FromBody] UpdateGlossaryCommandDto dto, Guid glossaryId)
    {
        try
        {
            UpdateGlossaryCommand command = new UpdateGlossaryCommand
            {
                Id = glossaryId,
                Key = dto.Key,
                Value = dto.Value,
                GlossaryLocalizations = dto.Localization.ToGlossaryLocalizationAppList()
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

    [Route(ApiRoutes.Glossary.GetAllForAdmin)]
    [HttpGet]
    [Permission(RoleConsistent.Glossary.BrowseGlossariesForAdmin)]
    public async Task<IActionResult> GetGlossariesForAdmin([FromQuery] GetGlossariesForAdminWithPaginationQuery query)
    {
        try
        {
            return Ok(await _sender.Send(query));
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Glossary.Get)]
    [HttpGet]
    [Permission(RoleConsistent.Glossary.BrowseGlossariesForAdmin)]
    public async Task<IActionResult> GetGlossaryForAdmin([FromRoute] GetGlossaryForAdminQuery query)
    {
        try
        {
            return Ok(await _sender.Send(query));
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }
}
