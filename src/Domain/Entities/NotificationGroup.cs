using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class NotificationGroup : BaseAuditableEntity
{
    public NotificationGroup() : base()
    {

    }
    public string Name { get; set; } = null!;
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<UserNotificationGroup> UserNotificationGroups { get; set; } = new List<UserNotificationGroup>();
}




