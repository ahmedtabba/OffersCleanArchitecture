using Microsoft.AspNetCore.SignalR;

namespace Offers.CleanArchitecture.Api;

public class ChatHub:Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}
