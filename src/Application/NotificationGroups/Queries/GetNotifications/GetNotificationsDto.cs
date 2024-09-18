using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotifications;
public class GetNotificationsDto : NotificationBaseDto
{

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Notification, GetNotificationsDto>();
        }
    }
}
