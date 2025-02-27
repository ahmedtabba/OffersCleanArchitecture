﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
public class GetAllUserNotificationsQueryValidator: AbstractValidator<GetAllUserNotificationsQuery>
{
    private readonly ILogger<GetAllUserNotificationsQueryValidator> _logger;

    public GetAllUserNotificationsQueryValidator(ILogger<GetAllUserNotificationsQueryValidator> logger)
    {
        RuleFor(x => x.PageNumber)
              .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(-1).WithMessage("PageSize at least greater than or equal to 1.");
        _logger = logger;
    }
}
