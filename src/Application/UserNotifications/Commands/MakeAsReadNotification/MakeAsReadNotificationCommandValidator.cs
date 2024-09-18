using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsReadNotification;
public class MakeAsReadNotificationCommandValidator : AbstractValidator<MakeAsReadNotificationCommand>
{
    private readonly ILogger<MakeAsReadNotificationCommandValidator> _logger;
    private readonly IUserNotificationRepository _userNotificationRepository;

    public MakeAsReadNotificationCommandValidator(ILogger<MakeAsReadNotificationCommandValidator> logger,
                                                  IUserNotificationRepository userNotificationRepository)
    {
        _logger = logger;
        _userNotificationRepository = userNotificationRepository;

        RuleFor(m => m.UserNotificationId)
            .NotEmpty().WithMessage("UserNotificationId must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserNotificationExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Make As Read Notification", "Notification is not found!");
                }
            });
    }

    public async Task<bool> IsUserNotificationExisted(MakeAsReadNotificationCommand command)
    {
        return await _userNotificationRepository.GetByIdAsync(command.UserNotificationId) != null;
    }
}
