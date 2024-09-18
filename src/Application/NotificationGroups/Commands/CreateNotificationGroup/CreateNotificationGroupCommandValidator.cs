using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Commands.CreateNotificationGroup;
public class CreateNotificationGroupCommandValidator : AbstractValidator<CreateNotificationGroupCommand>
{
    private readonly ILogger<CreateNotificationGroupCommandValidator> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly INotificationRepository _notificationRepository;

    public CreateNotificationGroupCommandValidator(ILogger<CreateNotificationGroupCommandValidator> logger,
                                                INotificationGroupRepository notificationGroupRepository,
                                                INotificationRepository notificationRepository)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;
        _notificationRepository = notificationRepository;

        RuleFor(n => n.Name)
            .NotEmpty().WithMessage("Notification Group must has Name")
            .MustAsync(BeUniqueName)
              .WithMessage("'{PropertyName}' must be unique")
              .WithErrorCode("Unique");

        RuleFor(n => n.NotificationsIds)
            .NotEmpty().WithMessage("Notification Group must has NotificationsIds")
            .CustomAsync(async (name, context, cancellationToken) =>
            {
                if (!await AreNotificationsIdsExisted(context.InstanceToValidate))
                {
                    context.AddFailure("NotificationsIds", "Some notifications are not found");
                }
            });

    }
    public async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return await _notificationGroupRepository.GetAll()
            .AllAsync(l => l.Name != name, cancellationToken);
    }

    public async Task<bool> AreNotificationsIdsExisted(CreateNotificationGroupCommand command)
    {
        foreach (var notificationId in command.NotificationsIds)
        {
            if (await _notificationRepository.GetByIdAsync(notificationId) == null)
            {
                return false;
            }
        }
        return true;
    }
}
