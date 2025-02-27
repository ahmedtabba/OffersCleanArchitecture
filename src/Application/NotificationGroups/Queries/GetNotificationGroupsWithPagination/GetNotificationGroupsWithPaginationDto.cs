﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries.GetNotificationGroupsWithPagination;
public class GetNotificationGroupsWithPaginationDto : NotificationGroupsBaseDto
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<NotificationGroup, GetNotificationGroupsWithPaginationDto>();
        }
    }
}
