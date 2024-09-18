using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Commands.UpdateNotificationGroup;
public class UpdateNotificationGroupCommandValidator : AbstractValidator<UpdateNotificationGroupCommand>
{
    private readonly ILogger<UpdateNotificationGroupCommandValidator> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly INotificationRepository _notificationRepository;

    public UpdateNotificationGroupCommandValidator(ILogger<UpdateNotificationGroupCommandValidator> logger,
                                                   INotificationGroupRepository notificationGroupRepository,
                                                   INotificationRepository notificationRepository)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;
        _notificationRepository = notificationRepository;

        RuleFor(n => n.Id)
            .NotEmpty().WithMessage("Notification Group must has Id")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await IsNotificationGroupExisted(context.InstanceToValidate))
                {
                    context.AddFailure("Update Notification Group", "NotificationGroupId must be correct");
                }
            });


        RuleFor(n => n.Name)
            .NotEmpty().WithMessage("Notification Group must has Name")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await BeUniqueName(context.InstanceToValidate))
                {
                    context.AddFailure("Update Notification Group", "NotificationGroup Name must be unique");
                }
            });

        RuleFor(n => n.NotificationsIds)
          .NotEmpty().WithMessage("Notification Group must has notifications")
          .CustomAsync(async (name, context, cancellationToken) =>
          {
              if (!await AreNotificationsIdsValid(context.InstanceToValidate))
              {
                  context.AddFailure("Update Notification Group", " Some notifications are not found");
              }
          });
    }
    public async Task<bool> IsNotificationGroupExisted(UpdateNotificationGroupCommand command)
    {
        return await _notificationGroupRepository.GetByIdAsync(command.Id) != null;
    }

    public async Task<bool> BeUniqueName(UpdateNotificationGroupCommand command)
    {
        return !await _notificationGroupRepository.GetAll()
            .AnyAsync(l => l.Name == command.Name && l.Id != command.Id);
    }

    public async Task<bool> AreNotificationsIdsValid(UpdateNotificationGroupCommand command)
    {
        foreach (var notificationId in command.NotificationsIds)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return false;
            }
        }
        return true;
    }

}
