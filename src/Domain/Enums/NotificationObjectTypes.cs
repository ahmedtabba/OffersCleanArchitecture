﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Enums;
public enum NotificationObjectTypes
{
    None,
    Grocery,
    Post,
    User,
    PermissionGroup,// => UserGroup
    NotificationGroup
}
