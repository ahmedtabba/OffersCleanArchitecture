using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Events.GroceryEvents;
using Offers.CleanArchitecture.Domain.Events.PostEvents;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Enums;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;

namespace Offers.CleanArchitecture.Application.Posts.EventHandlers;
public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
{
    private readonly ILogger<PostCreatedEventHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationGroupDetailRepository _notificationGroupDetailRepository;
    private readonly IUserNotificationGroupRepository _userNotificationGroupRepository;
    private readonly IUserNotificationService _userNotificationService;

    public PostCreatedEventHandler(ILogger<PostCreatedEventHandler> logger,
                                      INotificationService notificationService,
                                      IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                      INotificationGroupRepository notificationGroupRepository,
                                      INotificationRepository notificationRepository,
                                      INotificationGroupDetailRepository notificationGroupDetailRepository,
                                      IUserNotificationGroupRepository userNotificationGroupRepository,
                                      IUserNotificationService userNotificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _notificationGroupRepository = notificationGroupRepository;
        _notificationRepository = notificationRepository;
        _notificationGroupDetailRepository = notificationGroupDetailRepository;
        _userNotificationGroupRepository = userNotificationGroupRepository;
        _userNotificationService = userNotificationService;
    }

    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent} For Post :{PostTitle}", notification.GetType().Name, notification.Post.Title);

        // Get FavoriteGroceries record of the post's grocery, The returned list contains users'Ids who have the grocery as Favorite
        var favoriteGroceries = await _favoraiteGroceryRepository.GetAll()
            .Where(f => f.GroceryId == notification.Post.GroceryId)
            .ToListAsync();
        // usersIds is stored in token as NameIdentifier also, NameIdentifier used by SignalR to identify the user connected
        if (favoriteGroceries.Any()) 
        {
            var usersfavoriteIds = favoriteGroceries.Select(f => f.UserId).ToList();
            /*
            // get notificationGroups who have @"Post\Add Post"
            var addPostNotification = await _notificationRepository.GetAll()
                .Where(n => n.Name == NotificationConsistent.Post.Add)
                .SingleOrDefaultAsync();

            var addPostNotificationGroupsIds = await _notificationGroupDetailRepository.GetAll()
                .Where(d => d.NotificationId == addPostNotification.Id)
                .Select(d => d.NotificationGroupId)
                .ToListAsync();

            // get usersIds of the previous notificationGroups 
            var usersIds = await _userNotificationGroupRepository.GetAll()
                .Where(uNG => addPostNotificationGroupsIds.Contains(uNG.NotificationGroupId))
                .Select(uNG => uNG.UserId)
                .ToListAsync();

            // Intersection the usersId and usersfavoriteIds
            var usersToBeNotified = usersfavoriteIds.Intersect(usersIds).ToList();
            */
            // Push the notification to DB
            (string NotificationMessage, List<string> UsersIsd) signalRMessage = await _userNotificationService.Push(NotificationObjectTypes.Post, notification.Post.Id,
                                                                        NotificationConsistent.Post.Add,
                                                                           notificationMessage: $"Post {notification.Post.Title} has been added 🎉🎉",
                                                                           cancellationToken, usersfavoriteIds);
            
            // Send notification by SignalR
            if (signalRMessage.UsersIsd.Any())
            await _notificationService.SendFavoriteGroceryNotification(message: signalRMessage.NotificationMessage, signalRMessage.UsersIsd);
        }
        return; //Task.CompletedTask;
    }
}
