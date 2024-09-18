using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Api.NeededDto.OnboardingPage;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.OnboardingPages.Commands.CreateOnboardingPage;
using Offers.CleanArchitecture.Application.OnboardingPages.Commands.DeleteOnboardingPage;
using Offers.CleanArchitecture.Application.OnboardingPages.Commands.UpdateOnboardingPage;
using Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageForAdmin;
using Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPageLocalizationFieldType;
using Offers.CleanArchitecture.Application.OnboardingPages.Queries.GetOnboardingPagesWithPagination;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Controllers;
[ApiController]
public class OnboardingPagesController : ControllerBase
{
    private readonly ILogger<OnboardingPagesController> _logger;
    private readonly ISender _sender;

    public OnboardingPagesController(ILogger<OnboardingPagesController> logger,
                                     ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [Route(ApiRoutes.OnboardingPage.Create)]
    [HttpPost]
    [Permission(RoleConsistent.OnboardingPage.Add)]
    public async Task<IActionResult> CreateOnboardingPage([FromForm] CreateOnboardingPageCommandDto dto)
    {
        try
        {
            CreateOnboardingPageCommand command = new CreateOnboardingPageCommand
            {
                Title = dto.Title,
                Asset = dto.Asset != null ? dto.Asset.ToFileDto() : null!,
                Description = dto.Description,
                Order = dto.Order,
                OnboardingPageLocalizations = dto.OnboardingPageLocalizationDtos.ToOnboardingPageLocalizationAppList(),
                OnboardingPageLocalizationAssets = dto.LocalizedAssets.ToOnboardingPageLocalizationAssetsAppList(),
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

    [Route(ApiRoutes.OnboardingPage.Update)]
    [HttpPut]
    [Permission(RoleConsistent.OnboardingPage.Edit)]
    public async Task<IActionResult> UpdateOnboardingPage([FromForm] UpdateOnboardingPageCommandDto dto,[FromRoute] Guid onboardingPageId)
    {
        try
        {
            UpdateOnboardingPageCommand command = new UpdateOnboardingPageCommand
            {
                Id = onboardingPageId,
                Title = dto.Title,
                Description = dto.Description,
                Order = dto.Order,
                Asset = dto.Asset != null ? dto.Asset.ToFileDto() : null!,
                OnboardingPageLocalizations = dto.OnboardingPageLocalizationDtos.ToOnboardingPageLocalizationAppList(),
                OnboardingPageLocalizationAssets = dto.LocalizedAssets.ToOnboardingPageLocalizationAssetsAppList(),
                DeletedLocalizedAssetsIds = dto.DeletedLocalizedAssetsIds.ToList()

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

    [Route(ApiRoutes.OnboardingPage.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.OnboardingPage.Delete)]
    public async Task<IActionResult> DeleteOnboardingPage([FromRoute] DeleteOnboardingPageCommand command)
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

    [Route(ApiRoutes.OnboardingPage.Get)]
    [HttpGet]
    [Permission(RoleConsistent.OnboardingPage.BrowseOnboardingPageWithLocalization)]
    public async Task<IActionResult> GetOnboardingPage([FromRoute] GetOnboardingPageForAdminQuery query)
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

    [Route(ApiRoutes.OnboardingPage.GetAll)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetAllOnboardingPages([FromQuery] GetOnboardingPagesWithPaginationQuery query)
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

    [Route(ApiRoutes.OnboardingPage.GetOnboardingPageLocalizationFieldType)]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetOnboardingPageLocalizationFieldType()
    {
        try
        {
            GetOnboardingPageLocalizationFieldTypeQuery query = new GetOnboardingPageLocalizationFieldTypeQuery();
            return Ok(await _sender.Send(query));
        }
        catch (Exception ex)
        {
            List<string> messages = JsonParser.ParseMessages(ex.Message);
            return BadRequest(new { Errors = messages });
        }
    }
}
