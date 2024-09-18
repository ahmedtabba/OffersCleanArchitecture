using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class NotificationGroupDetail : BaseAuditableEntity
{
    public NotificationGroupDetail() : base()
    {

    }
    public Guid NotificationGroupId { get; set; }
    public Guid NotificationId { get; set; }
}
