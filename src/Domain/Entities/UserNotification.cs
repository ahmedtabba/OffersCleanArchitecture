using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class UserNotification : BaseAuditableEntity
{
    public UserNotification() : base()
    {

    }
    public Guid NotificationId { get; set; }
    public virtual Notification Notification { get; set; } = new Notification();
    public string UserId { get; set; } = null!;
    public string NotificationString { get; set; } = null!;
    public Guid ObjectId { get; set; }
    public int ObjectType { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTimeOffset NotificationDate { get; set; }
}
