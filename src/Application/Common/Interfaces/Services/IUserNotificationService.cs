using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Services;
public interface IUserNotificationService
{
    public Task<(string NotificationMessage, List<string> UsersIsd)> Push(NotificationObjectTypes objectType, Guid ObjectId, string notificationConst, string notificationMessage, CancellationToken cancellationToken, List<string>? userFavoriteIds);
    public Task MakeAsReadNotification(Guid userNotificationId, CancellationToken cancellationToken);
    public Task MakeAsUnReadNotification(Guid userNotificationId, CancellationToken cancellationToken);
    public Task MakeAllAsReadNotification(string userId, CancellationToken cancellationToken);
    public Task<List<GetAllUserNotificationsDto>> GetAllUserNotifications(string userId, bool? isRead);
    public IQueryable<UserNotification> GetAllUserNotificationsQueryable(string userId, bool? isRead);
}
