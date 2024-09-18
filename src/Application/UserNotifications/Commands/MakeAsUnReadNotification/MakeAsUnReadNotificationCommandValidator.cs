using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.UserNotifications.Commands.MakeAsUnReadNotification;
public class MakeAsUnReadNotificationCommandValidator : AbstractValidator<MakeAsUnReadNotificationCommand>
{
    private readonly ILogger<MakeAsUnReadNotificationCommandValidator> _logger;
    private readonly IUserNotificationRepository _userNotificationRepository;

    public MakeAsUnReadNotificationCommandValidator(ILogger<MakeAsUnReadNotificationCommandValidator> logger,
                                                  IUserNotificationRepository userNotificationRepository)
    {
        _logger = logger;
        _userNotificationRepository = userNotificationRepository;

        RuleFor(m => m.UserNotificationId)
            .NotEmpty().WithMessage("UserNotificationId Must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsUserNotificationExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Make As UnRead Notification", "Notification is not found!");
                }
            });
    }

    public async Task<bool> IsUserNotificationExisted(MakeAsUnReadNotificationCommand command)
    {
        return await _userNotificationRepository.GetByIdAsync(command.UserNotificationId) != null;
    }
}
