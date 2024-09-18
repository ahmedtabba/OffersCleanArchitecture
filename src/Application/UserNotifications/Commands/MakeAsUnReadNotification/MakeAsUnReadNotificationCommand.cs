using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsUnReadNotification;
public class MakeAsUnReadNotificationCommand : IRequest
{
    public Guid UserNotificationId { get; set; }
}

public class MakeAsUnReadNotificationCommandHandler : IRequestHandler<MakeAsUnReadNotificationCommand>
{
    private readonly ILogger<MakeAsUnReadNotificationCommandHandler> _logger;
    private readonly IUserNotificationService _userNotificationService;

    public MakeAsUnReadNotificationCommandHandler(ILogger<MakeAsUnReadNotificationCommandHandler> logger,
                                                IUserNotificationService userNotificationService)
    {
        _logger = logger;
        _userNotificationService = userNotificationService;
    }

    public async Task Handle(MakeAsUnReadNotificationCommand request, CancellationToken cancellationToken)
    {
        await _userNotificationService.MakeAsUnReadNotification(request.UserNotificationId, cancellationToken);
    }
}
