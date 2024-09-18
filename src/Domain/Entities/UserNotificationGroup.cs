using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class UserNotificationGroup : BaseAuditableEntity
{
    public UserNotificationGroup() : base()
    {

    }
    public Guid NotificationGroupId { get; set; }
    public virtual NotificationGroup NotificationGroup { get; set; } = new NotificationGroup();
    public string UserId { get; set; } = null!;
}
