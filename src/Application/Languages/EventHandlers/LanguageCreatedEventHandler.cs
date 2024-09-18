using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Domain.Events.LanguageEvents;

namespace Offers.CleanArchitecture.Application.Languages.EventHandlers;
public class LanguageCreatedEventHandler : INotificationHandler<LanguageCreatedEvent>
{
    private readonly ILogger<LanguageCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public LanguageCreatedEventHandler(ILogger<LanguageCreatedEventHandler> logger,
                                       INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    public Task Handle(LanguageCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent} For Language :{LanguageName}", notification.GetType().Name, notification.Language.Name);
        _notificationService.SendNotification(message: $"Language {notification.Language.Name} has been added 🎉🎉");
        return Task.CompletedTask;
    }
}
