using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsReadNotification;
public class MakeAsReadNotificationCommand : IRequest
{
    public Guid UserNotificationId { get; set; }
}

public class MakeAsReadNotificationCommandHandler : IRequestHandler<MakeAsReadNotificationCommand>
{
    private readonly ILogger<MakeAsReadNotificationCommandHandler> _logger;
    private readonly IUserNotificationService _userNotificationService;

    public MakeAsReadNotificationCommandHandler(ILogger<MakeAsReadNotificationCommandHandler> logger,
                                                IUserNotificationService userNotificationService)
    {
        _logger = logger;
        _userNotificationService = userNotificationService;
    }

    public async Task Handle(MakeAsReadNotificationCommand request, CancellationToken cancellationToken)
    {
        await _userNotificationService.MakeAsReadNotification(request.UserNotificationId, cancellationToken);
    }
}
