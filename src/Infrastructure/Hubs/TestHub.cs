using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Offers.CleanArchitecture.Infrastructure.Hubs;
//unused hup, but in future may be useful or may we edit it to do some notifications
public class TestHub : Hub
{
    public static int TotalViews {  get; set; } = 0;
    public static int TotalUsers {  get; set; } = 0;

    public override Task OnConnectedAsync()
    {
        TotalUsers++;
        Clients.All.SendAsync("updateTotalUsers",TotalUsers).GetAwaiter().GetResult();
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        TotalUsers--;
        Clients.All.SendAsync("updateTotalUsers",TotalUsers).GetAwaiter().GetResult();
        return base.OnDisconnectedAsync(exception);
    }

    public async Task NewWindowLoaded()
    {
        TotalViews ++;
        await Clients.All.SendAsync("updateTotalViews", TotalViews);
    }
}
