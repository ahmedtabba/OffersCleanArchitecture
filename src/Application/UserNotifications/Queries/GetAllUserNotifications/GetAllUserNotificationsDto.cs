using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
public class GetAllUserNotificationsDto : UserNotificationBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UserNotification, GetAllUserNotificationsDto>()
                .ForMember(dto => dto.NotificationObjectType, src => src.MapFrom(s => (NotificationObjectTypes)s.ObjectType));
        }
    }
}
