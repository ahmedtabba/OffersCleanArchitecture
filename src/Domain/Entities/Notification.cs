using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class Notification : BaseAuditableEntity
{
    public Notification() : base()
    {

    }
    public string Name { get; set; } = null!;
    public virtual ICollection<NotificationGroup> NotificationGroups { get; set; } = new List<NotificationGroup>();
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}




