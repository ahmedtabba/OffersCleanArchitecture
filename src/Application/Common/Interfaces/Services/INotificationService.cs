using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Services;
public interface INotificationService
{
    Task SendNotification(string message, string? user = null);
    Task SendFavoriteGroceryNotification(string message, List<string> usersIds);
}

