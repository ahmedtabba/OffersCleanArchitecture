using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Events.GroceryEvents;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Domain.Events.GroceryLocalizationEvents;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Application.Groceries.EventHandlers;
public class GroceryLocalizationCreatedEventHandler : INotificationHandler<GroceryLocalizationCreatedEvent>
{
    private readonly ILogger<GroceryLocalizationCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IGroceryRepository _groceryRepository;

    public GroceryLocalizationCreatedEventHandler(ILogger<GroceryLocalizationCreatedEventHandler> logger,
                                      INotificationService notificationService,
                                      IGroceryRepository groceryRepository)
    {
        _logger = logger;
        _notificationService = notificationService;
        _groceryRepository = groceryRepository;
    }
    public async Task Handle(GroceryLocalizationCreatedEvent notification, CancellationToken cancellationToken)
    {
        var grocery = await _groceryRepository.GetByIdAsync(notification.GroceryLocalization.GroceryId);
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent} For GroceryLocalizationType :{GroceryLocalizationType} for Grocery {GroceryName}",
            notification.GetType().Name,(GroceryLocalizationFieldType) notification.GroceryLocalization.GroceryLocalizationFieldType,grocery.Name);

        await _notificationService.SendNotification(message: $"GroceryLocalization {(GroceryLocalizationFieldType)notification.GroceryLocalization.GroceryLocalizationFieldType} for {grocery.Name} has been added 🎉🎉");
        //return Task.FromResult( Task.CompletedTask);
    }
}
