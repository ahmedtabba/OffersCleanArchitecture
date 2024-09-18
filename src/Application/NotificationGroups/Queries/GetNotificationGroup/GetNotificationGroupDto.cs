using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotifications;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroup;
public class GetNotificationGroupDto : NotificationGroupsBaseDto
{
    public ICollection<NotificationGroupNotificationDto> Notifications { get; set; } = new List<NotificationGroupNotificationDto>();

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<NotificationGroup, GetNotificationGroupDto>();
        }
    }
}
