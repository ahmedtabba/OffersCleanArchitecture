using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Commands.DeleteNotificationGroup;
public class DeleteNotificationGroupCommandValidator : AbstractValidator<DeleteNotificationGroupCommand>
{
    private readonly ILogger<DeleteNotificationGroupCommandValidator> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly IUserNotificationGroupRepository _userNotificationGroupRepository;

    public DeleteNotificationGroupCommandValidator(ILogger<DeleteNotificationGroupCommandValidator> logger,
                                                   INotificationGroupRepository notificationGroupRepository,
                                                   IUserNotificationGroupRepository userNotificationGroupRepository)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;
        _userNotificationGroupRepository = userNotificationGroupRepository;

        RuleFor(n => n.NotificationGroupId)
            .NotEmpty().WithMessage("Id must be passed")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await CanDeleteNotificationGroup(context.InstanceToValidate))
                {
                    context.AddFailure("Delete NotificationGroup", "Notification Group is not found or there are users with this Group!");
                }
            });
    }

    public async Task<bool> CanDeleteNotificationGroup(DeleteNotificationGroupCommand command)
    {
        var notificationGroup = await _notificationGroupRepository.GetByIdAsync(command.NotificationGroupId);
        if (notificationGroup == null)
        {
            return false;
        }
        var existedNotificationGroupUser = await _userNotificationGroupRepository.GetAll()
            .AnyAsync(n => n.NotificationGroupId == command.NotificationGroupId);
        if (existedNotificationGroupUser)
        {
            return false;
        }

        return true;
    }

}
