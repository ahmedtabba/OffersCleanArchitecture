using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Events.GroceryEvents;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Application.Groceries.EventHandlers;
public class GroceryCreatedEventHandler : INotificationHandler<GroceryCreatedEvent>
{
    private readonly ILogger<GroceryCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public GroceryCreatedEventHandler(ILogger<GroceryCreatedEventHandler> logger,
                                      INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }
    public Task Handle(GroceryCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent} For Grocery :{GroceryName}", notification.GetType().Name, notification.Grocery.Name);

        _notificationService.SendNotification(message: $"Grocery {notification.Grocery.Name} has been added 🎉🎉");
        return Task.CompletedTask;
    }
}
