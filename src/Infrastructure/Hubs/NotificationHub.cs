using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Hubs;
[Authorize]
public sealed class NotificationHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
    private readonly IIdentityService _identityService;

    public NotificationHub(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext != null)
        {
            var jwtToken = httpContext.Request.Query["access_token"];
            var handler = new JwtSecurityTokenHandler();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                var token = handler.ReadJwtToken(jwtToken);
                var tokenS = token as JwtSecurityToken;

                // Replace "email" with your claim name
                var jti = tokenS.Claims.First(claim => claim.Type == ClaimTypes.SerialNumber).Value;//SerialNumber represent UserId
                if (jti != null && jti != "")
                {
                    var userName = _identityService.GetAllUsers().Where(x => x.Id == jti).Select(x => x.UserName).FirstOrDefault();
                    //we can get userName form token directly from ClaimTypes.Name
                    //var userName = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
                    // Use jti as needed, e.g., to add the user to a group
                    _connections.TryAdd(Context.ConnectionId, userName!);
                }
            }
        }
        await Clients.All.SendAsync("ReceiveMessage", $"{_connections[Context.ConnectionId]} has joined 😊");
        //await base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connections.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotification(string message, string? user)
    {
        if (user != null)
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        else
            await Clients.All.SendAsync("ReceiveNotification", message);

    }

}
