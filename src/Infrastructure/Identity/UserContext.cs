using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Offers.CleanArchitecture.Application.Common.Interfaces;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool CheckIfUserAuthorized()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        return  request.Headers.TryGetValue("Authorization", out var token);
    }

    public Guid GetCountryIdOfUser()
    {
        var countryId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "country_Id").Value;
        if (countryId == null)
            return Guid.Empty;
        return Guid.Parse(countryId);
    }
    public Guid GetLanguageIdOfUser()
    {
        var languageId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Locality).Value;
        if (languageId == null)
            return Guid.Empty;
        return Guid.Parse(languageId);
    }
        

}
