using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Languages.EventHandlers;
using Offers.CleanArchitecture.Domain.Events.CountryEvents;
using Offers.CleanArchitecture.Domain.Events.LanguageEvents;

namespace Offers.CleanArchitecture.Application.Countries.EventHandlers;
public class CountryCreatedEventHandler : INotificationHandler<CountryCreatedEvent>
{
    private readonly ILogger<CountryCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public CountryCreatedEventHandler(ILogger<CountryCreatedEventHandler> logger,
                                       INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    public Task Handle(CountryCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent} For Country :{CountryName}", notification.GetType().Name, notification.Country.Name);
        _notificationService.SendNotification(message: $"Country {notification.Country.Name} has been added 🎉🎉");
        return Task.CompletedTask;
    }
}
