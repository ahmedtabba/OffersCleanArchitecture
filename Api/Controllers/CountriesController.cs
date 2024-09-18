using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.Country;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Common.Exceptions;
using Offers.CleanArchitecture.Application.Countries.Commands.CreateCountry;
using Offers.CleanArchitecture.Application.Countries.Commands.DeleteCountry;
using Offers.CleanArchitecture.Application.Countries.Commands.UpdateCountry;
using Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;
using Offers.CleanArchitecture.Application.Countries.Queries.GetCountryQuery;
using Offers.CleanArchitecture.Application.Countries.Queries.GetTimeZones;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Controllers;
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(ISender sender,
                               ILogger<CountriesController> logger)
    {
        _sender = sender;
        _logger = logger;
    }


    [Route(ApiRoutes.Country.GetAll)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetCountriesWithPagination([FromQuery] GetCountriesWithPaginationQuery query)
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

    [Route(ApiRoutes.Country.Get)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetCountry([FromRoute] GetCountryQuery query)
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

    [Route(ApiRoutes.Country.Create)]
    [HttpPost]
    [Permission(RoleConsistent.Country.Add)]
    public async Task<IActionResult> CreateCountry([FromForm] CreateCountryCommandDto dto)
    {
        try
        {
            CreateCountryCommand command = new CreateCountryCommand
            {
                Name = dto.Name,
                Flag = dto.Flag != null ? dto.Flag.ToFileDto() : null!,
                TimeZoneId = dto.TimeZoneId,
                Code = dto.Code,
            };
            var result = await _sender.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }

    [Route(ApiRoutes.Country.Update)]
    [HttpPut]
    [Permission(RoleConsistent.Country.Edit)]
    public async Task<IActionResult> UpdateCountry([FromForm] UpdateCountryCommandDto dto, Guid countryId)
    {
        try
        {
            UpdateCountryCommand command = new UpdateCountryCommand
            {
                Id = countryId,
                Name = dto.Name,
                Flag = dto.Flag != null ? dto.Flag.ToFileDto() : null,
                TimeZoneId = dto.TimeZoneId,
                Code = dto.Code,
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

    [Route(ApiRoutes.Country.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.Country.Delete)]
    public async Task<IActionResult> DeleteCountry([FromRoute] DeleteCountryCommand command)
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

    [Route(ApiRoutes.Country.GetAllTimeZones)]
    [HttpGet]
    [Permission(RoleConsistent.Country.BrowseTimeZones)]
    public async Task<IActionResult> GetTimeZones([FromQuery] GetTimeZonesQuery query)
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
}
