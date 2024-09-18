using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.UserNotifications.Queries;
public class UserNotificationBaseDto
{
    public Guid Id { get; set; }
    public Guid NotificationId { get; set; }
    public string UserId { get; set; } = null!;
    public string NotificationString { get; set; } = null!;
    public Guid ObjectId { get; set; }
    public NotificationObjectTypes NotificationObjectType { get; set; }
    public string NotificationObjectTypeDescription => NotificationObjectType.ToString();
    public bool IsRead { get; set; } = false;
    public DateTimeOffset NotificationDate { get; set; }
}
