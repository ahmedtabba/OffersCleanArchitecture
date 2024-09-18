using System.Text.RegularExpressions;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.Grocery;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Common.Exceptions;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Groceries.Commands.CreateGrocery;
using Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;
using Offers.CleanArchitecture.Application.Groceries.Commands.UpdateGrocery;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.AddToFavoraite;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.GetUserFavoraites;
using Offers.CleanArchitecture.Application.Groceries.Favoraite.RemoveFromFavoraite;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesForAdminWithPagination;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryLocalizationFieldType;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Controllers;

[ApiController]
public class GroceriesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<GroceriesController> _logger;
    private readonly IUserContext _userContext;

    public GroceriesController(ISender sender,
                               ILogger<GroceriesController> logger,
                               IUserContext userContext)
    {
        _sender = sender;
        _logger = logger;
        _userContext = userContext;
    }

    [Route(ApiRoutes.Grocery.GetAll)]
    [HttpGet]
    public async Task<IActionResult> GetGroceriesWithPagination([FromQuery] GetGroceriesWithPaginationUnAuthorizedDto dto)
    {
        try
        {
            (Guid countryId, Guid languageId) = UnAuthorizedUserCheeker.GetCountryAndLanguageId(dto.CountryId, dto.LanguageId, _userContext);
            GetGroceriesWithPaginationQuery query = new GetGroceriesWithPaginationQuery
            {
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                CountryId = countryId,
                LanguageId = languageId,
                SearchText = dto.SearchText,
            };

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

    [Route(ApiRoutes.Grocery.GetAllForAdmin)]
    [HttpGet]
    [Permission(RoleConsistent.Grocery.Browse)]
    public async Task<IActionResult> GetGroceriesForAdmin([FromQuery] GetGroceriesForAdminWithPaginationQuery query)
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

    [Route(ApiRoutes.Grocery.Get)]
    [HttpGet]
    public async Task<IActionResult> GetGrocery([FromRoute] Guid groceryId , [FromQuery] GetGroceryUnAuthorizedDto dto)
    {
        try
        {
            (Guid countryId, Guid languageId) = UnAuthorizedUserCheeker.GetCountryAndLanguageId(dto.CountryId, dto.LanguageId, _userContext);
            GetGroceryQuery query = new GetGroceryQuery
            {
                GroceryId = groceryId,
                CountryId = countryId,
                LanguageId = languageId,
            };
            var res = await _sender.Send(query);
            return Ok(res);
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

    [Route(ApiRoutes.Grocery.GetForAdmin)]
    [HttpGet]
    [Permission(RoleConsistent.Grocery.Browse)]
    public async Task<IActionResult> GetGroceryForAdmin([FromRoute] GetGroceryForAdminQuery query)
    {
        try
        {
            var res = await _sender.Send(query);
            return Ok(res);
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

    [Route(ApiRoutes.Grocery.Create)]
    [HttpPost]
    [Permission(RoleConsistent.Grocery.Add)]
    public async Task<IActionResult> CreateGrocery([FromForm] CreateGroceryCommandDto dto)
    {
        try
        {
            // map CreateGroceryCommandDto => CreateGroceryCommand to pass IFormFile as FileDto (Stream)
            CreateGroceryCommand command = new CreateGroceryCommand
            {
                CountryId = dto.CountryId,
                Address = dto.Address,
                Name = dto.Name,
                Description = dto.Description,
                File = dto.Logo != null ? dto.Logo.ToFileDto() : null!,
                GroceryLocalizations = dto.GroceryLocalizationDtos.ToGroceryLocalizationAppList()
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

    [Route(ApiRoutes.Grocery.Update)]
    [HttpPut]
    [Permission(RoleConsistent.Grocery.Edit)]
    public async Task<IActionResult> UpdateGrocery([FromForm] UpdateGroceryCommandDto dto, Guid groceryId)
    {
        try 
        {
            // map UpdateGroceryCommandDto => UpdateGroceryCommand
            UpdateGroceryCommand command = new UpdateGroceryCommand
            {
                CountryId = dto.CountryId,
                Id = groceryId,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                File = dto.Logo is not null ? dto.Logo.ToFileDto() : null!,
                GroceryLocalizations = dto.GroceryLocalizationDtos.ToGroceryLocalizationAppList()
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

    [Route(ApiRoutes.Grocery.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.Grocery.Delete)]
    public async Task<IActionResult> DeleteGrocery([FromRoute] DeleteGroceryCommand command)
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

    [Route(ApiRoutes.Grocery.AddToFavoraite)]
    [HttpPost]
    [Permission(RoleConsistent.Grocery.AddToFavorite)]
    public async Task<IActionResult> AddToFavoraite([FromRoute] AddToFavoraiteCommand command)
    {
        try
        {
            await _sender.Send(command);
            return NoContent() ;
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }
    [Route(ApiRoutes.Grocery.RemoveFromFavoraite)]
    [HttpDelete]
    [Permission(RoleConsistent.Grocery.RemoveFromFavorite)]
    public async Task<IActionResult> RemoveFromFavoraite([FromRoute] RemoveFromFavoraiteCommand command)
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

    [Route(ApiRoutes.Grocery.GetUserFavoraites)]
    [HttpGet]
    [Permission(RoleConsistent.Grocery.BrowseFavorite)]
    public async Task<IActionResult> GetUserFavoraitesWithPagination([FromQuery] GetUserFavoraitesWithPaginationQuery query)
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

    [Route(ApiRoutes.Grocery.GetGroceryLocalizationFieldType)]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetGroceryLocalizationFieldType()
    {
        try
        {
            GetGroceryLocalizationFieldTypeQuery query = new GetGroceryLocalizationFieldTypeQuery();
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
