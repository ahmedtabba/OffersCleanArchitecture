using System.ComponentModel.DataAnnotations;
using Offers.CleanArchitecture.Api.NeededDto.Post;
using Offers.CleanArchitecture.Api.Utilities;
using Offers.CleanArchitecture.Application.Posts.Commands.CreatePost;
using Offers.CleanArchitecture.Application.Posts.Commands.DeletePost;
using Offers.CleanArchitecture.Application.Posts.Commands.UpdatePost;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostQuery;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByFavoriteGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsForAdminQueryWithPagination;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostForAdminQuery;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostFilters;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostLocalizationFieldType;
using Offers.CleanArchitecture.Api.Helpers;
using Offers.CleanArchitecture.Infrastructure.Utilities;

namespace Offers.CleanArchitecture.Api.Controllers;

[ApiController]
public class PostsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<PostsController> _logger;
    private readonly IUserContext _userContext;

    public PostsController(ISender sender,
                           ILogger<PostsController> logger,
                           IUserContext userContext)
    {
        _sender = sender;
        _logger = logger;
        _userContext = userContext;
    }

    [Route(ApiRoutes.Post.Create)]
    [HttpPost]
    [Permission(RoleConsistent.Post.Add)]
    public async Task<IActionResult> CreatePost([FromForm] CreatePostCommandDto dto, [FromRoute] Guid groceryId)
    {
        try
        {
            // map CreatePostCommandDto => CreatePostCommand to pass IFormFile as FileDto (Stream)
            CreatePostCommand command = new CreatePostCommand
            {
                GroceryId = groceryId,
                Title = dto.Title,
                Description = dto.Description,
                IsActive = dto.IsActive,
                StartDate = dto.StartDate,
                PublishDate = dto.PublishDate,
                EndDate = dto.EndDate,
                File = dto.Asset.ToFileDto(),
                PostLocalizations = dto.PostLocalizationDtos.ToPostLocalizationAppList(),
                PostLocalizationImages = dto.LocalizedAssets.ToPostLocalizationAssetsAppList(),
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

    [Route(ApiRoutes.Post.Update)]
    [HttpPut]
    [Permission(RoleConsistent.Post.Edit)]
    public async Task<IActionResult> UpdatePost([FromForm] UpdatePostCommandDto dto, Guid postId)
    {
        try
        {
            // map UpdatePostCommandDto => UpdatePostCommand
            UpdatePostCommand command = new UpdatePostCommand
            {
                Id = postId,
                Title = dto.Title,
                IsActive = dto.IsActive,
                Description = dto.Description,
                PublishDate = dto.PublishDate,
                EndDate = dto.EndDate,
                StartDate = dto.StartDate,
                PostLocalizations = dto.PostLocalizationDtos.ToPostLocalizationAppList(),
                PostLocalizationImages = dto.LocalizedAssets.ToPostLocalizationAssetsAppList(),
                DeletedLocalizedAssetsIds = dto.DeletedLocalizedAssetsIds.ToList(),
                File = dto.Asset is not null ? dto.Asset.ToFileDto() : null!
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

    [Route(ApiRoutes.Post.Delete)]
    [HttpDelete]
    [Permission(RoleConsistent.Post.Delete)]
    public async Task<IActionResult> DeletePost([FromRoute] DeletePostCommand command)
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

    [Route(ApiRoutes.Post.GetAllByGrocery)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetPostsByGroceryWithPagination([FromRoute] Guid groceryId,[FromQuery] GetPostsByGroceryWithPaginationQueryDto dto)
    {
        (Guid countryId, Guid languageId) = UnAuthorizedUserCheeker.GetCountryAndLanguageId(dto.CountryId, dto.LanguageId, _userContext);
        GetPostsByGroceryWithPaginationQuery query = new GetPostsByGroceryWithPaginationQuery
        {
            GroceryId = groceryId,
            PageNumber = dto.PageNumber,
            PageSize = dto.PageSize,
            SearchText = dto.SearchText,
            CountryId = countryId,
            LanguageId = languageId,
            PostFilter = dto.PostFilter.ToPostFilter()
        };
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

    [Route(ApiRoutes.Post.GetAllForAdmin)]
    [HttpGet]
    [Permission(RoleConsistent.Post.BrowseForAdmin)]
    public async Task<IActionResult> GetPostsForAdminWithPagination([FromQuery] GetPostsForAdminWithPaginationQuery query)
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
    [Route(ApiRoutes.Post.GetAllPosts)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetPostsWithPagination([FromQuery] GetPostsWithPaginationUnAuthorizedDto dto)
    {
        try
        {
            (Guid countryId, Guid languageId) = UnAuthorizedUserCheeker.GetCountryAndLanguageId(dto.CountryId, dto.LanguageId, _userContext);
            GetPostsWithPaginationQuery query = new GetPostsWithPaginationQuery
            {
                CountryId = countryId,
                LanguageId = languageId,
                PostFilter = dto.PostFilter.ToPostFilter(),
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                SearchText = dto.SearchText
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

    [Route(ApiRoutes.Post.GetAllFavoritePosts)]
    [HttpGet]
    [Permission(RoleConsistent.Post.BrowseFavoritePosts)]
    public async Task<IActionResult> GetPostsByFavoriteGroceriesWithPagination([FromQuery] GetPostsByFavoriteGroceriesWithPaginationQueryDto dto)
    {
        try
        {
            GetPostsByFavoriteGroceriesWithPaginationQuery query = new GetPostsByFavoriteGroceriesWithPaginationQuery
            {
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize,
                SearchText = dto.SearchText,
                PostFilter = dto.PostFilter.ToPostFilter()
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

    [Route(ApiRoutes.Post.Get)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetPostById([FromRoute] Guid postId, [FromQuery] GetPostUnAuthorizedDto dto)
    {
        try
        {
            (Guid countryId, Guid languageId) = UnAuthorizedUserCheeker.GetCountryAndLanguageId(dto.CountryId, dto.LanguageId, _userContext);
            GetPostQuery query = new GetPostQuery
            {
                postId = postId,
                CountryId = countryId,
                LanguageId = languageId,
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

    [Route(ApiRoutes.Post.GetForAdmin)]
    [HttpGet]
    [Permission(RoleConsistent.Post.BrowseForAdmin)]
    public async Task<IActionResult> GetPostForAdminById([FromRoute] GetPostForAdminQuery query)
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

    [Route(ApiRoutes.Post.GetFilters)]
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetFilters()
    {
        try
        {
            GetPostFiltersQuery query = new GetPostFiltersQuery();
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

    [Route(ApiRoutes.Post.GetPostLocalizationFieldType)]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPostLocalizationFieldType()
    {
        try
        {
            GetPostLocalizationFieldTypeQuery query = new GetPostLocalizationFieldTypeQuery();
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
