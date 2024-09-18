using System.Security.Claims;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Api.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    //public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id");
    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.SerialNumber);
}
