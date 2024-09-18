using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAllAsReadNotification;
public class MakeAllAsReadNotificationCommand : IRequest
{
    //public string UserId { get; set; } = null!;
}

public class MakeAllAsReadNotificationCommandHandler : IRequestHandler<MakeAllAsReadNotificationCommand>
{
    private readonly ILogger<MakeAllAsReadNotificationCommandHandler> _logger;
    private readonly IUserNotificationService _userNotificationService;
    private readonly IUser _user;

    public MakeAllAsReadNotificationCommandHandler(ILogger<MakeAllAsReadNotificationCommandHandler> logger,
                                                   IUserNotificationService userNotificationService,
                                                   IUser user)
    {
        _logger = logger;
        _userNotificationService = userNotificationService;
        _user = user;
    }

    public async Task Handle(MakeAllAsReadNotificationCommand request, CancellationToken cancellationToken)
    {
        await _userNotificationService.MakeAllAsReadNotification(_user.Id,cancellationToken);
    }
}
