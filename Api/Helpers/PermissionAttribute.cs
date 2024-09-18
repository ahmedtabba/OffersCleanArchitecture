using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Offers.CleanArchitecture.Api.Helpers;

public class PermissionAttribute : AuthorizeAttribute
{
    public PermissionAttribute(params string[] roles) : base()
    {

        Roles = string.Join(",", roles);
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
    }

}
