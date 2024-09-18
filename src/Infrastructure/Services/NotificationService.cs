using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Infrastructure.Services;
public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendFavoriteGroceryNotification(string message, List<string> usersIds)
    {
        await _hubContext.Clients.Users(usersIds).SendAsync("ReceiveFavoriteNotification", message);
        //await _hubContext.Clients.All.SendAsync("ReceiveFavoriteNotification", message);
    }

    public async Task SendNotification( string message, string? user)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", user, message);
    }
}
