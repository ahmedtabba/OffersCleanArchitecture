using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.UserNotifications.Queries.GetAllUserNotifications;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Infrastructure.Services;
public class UserNotificationService : IUserNotificationService
{
    private readonly ILogger<UserNotificationService> _logger;
    private readonly INotificationGroupRepository _notificationGroupRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationGroupDetailRepository _notificationGroupDetailRepository;
    private readonly IUserNotificationGroupRepository _userNotificationGroupRepository;
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly IMapper _mapper;

    public UserNotificationService(ILogger<UserNotificationService> logger,
                                   INotificationGroupRepository notificationGroupRepository,
                                   INotificationRepository notificationRepository,
                                   INotificationGroupDetailRepository notificationGroupDetailRepository,
                                   IUserNotificationGroupRepository userNotificationGroupRepository,
                                   IUserNotificationRepository userNotificationRepository,
                                   IUnitOfWorkAsync unitOfWork,
                                   IMapper mapper)
    {
        _logger = logger;
        _notificationGroupRepository = notificationGroupRepository;
        _notificationRepository = notificationRepository;
        _notificationGroupDetailRepository = notificationGroupDetailRepository;
        _userNotificationGroupRepository = userNotificationGroupRepository;
        _userNotificationRepository = userNotificationRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GetAllUserNotificationsDto>> GetAllUserNotifications(string userId, bool? isRead)
    {
        var notifications = _userNotificationRepository.GetAll();
        if (userId != null)
        {
            notifications = notifications
                .Where(n => n.UserId == userId);
        }
        if (isRead != null)
        {
            notifications = notifications
                .Where(n=>n.IsRead == isRead);
        }

        var result = await notifications
            .OrderBy(n=>n.NotificationDate)
            .ProjectTo<GetAllUserNotificationsDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return result;
    }
    public IQueryable<UserNotification> GetAllUserNotificationsQueryable(string userId, bool? isRead)
    {
        var notifications = _userNotificationRepository.GetAll();
        if (userId != null)
        {
            notifications = notifications
                .Where(n => n.UserId == userId);
        }
        if (isRead != null)
        {
            notifications = notifications
                .Where(n => n.IsRead == isRead);
        }
        return notifications;
    }
    public async Task MakeAsReadNotification(Guid userNotificationId,CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var userNotification = await _userNotificationRepository.GetByIdAsync(userNotificationId);
            userNotification.IsRead = true;
            await _userNotificationRepository.UpdateAsync(userNotification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        
    }

    public async Task MakeAllAsReadNotification(string userId, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var userNotifications = await _userNotificationRepository.GetAllAsTracking()
                .Where(n => n.IsRead == false && n.UserId == userId)
                .ToListAsync();
            foreach (var userNotification in userNotifications)
            {
                userNotification.IsRead = true;
                await _userNotificationRepository.UpdateAsync(userNotification);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

    }

    public async Task MakeAsUnReadNotification(Guid userNotificationId, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var userNotification = await _userNotificationRepository.GetByIdAsync(userNotificationId);
            userNotification.IsRead = false;
            await _userNotificationRepository.UpdateAsync(userNotification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<(string NotificationMessage, List<string> UsersIsd)> Push(NotificationObjectTypes objectType, Guid ObjectId, string notificationConst,
                                                                                string notificationMessage, CancellationToken cancellationToken,
                                                                                List<string>? specificNotifiedUsers)
    {
        // get notificationGroups who have notificationConst
        var notificationGroupsResult = await GetNotificationGroups(notificationConst);

        // get usersIds of the previous notificationGroups
        var usersIds = new List<string>();
        //check if there are Notification Groups of notificationConst
        if (notificationGroupsResult.NotificationGroupsIds.Any())
        {
            usersIds = await GetUsersIdsOfNotificationGroups(notificationGroupsResult.NotificationGroupsIds);
        }
        

        // Intersection the usersIds and userFavoriteIds
        List<string>? usersToBeNotified;
        if (specificNotifiedUsers != null && specificNotifiedUsers.Any())
        {
            // if there are no Notification Groups of notificationConst => usersIds.Count = 0, so usersToBeNotified will be empty
            usersToBeNotified = specificNotifiedUsers.Intersect(usersIds).ToList();
        }
        else
        {
            usersToBeNotified = usersIds;
        }



        // Insert Notifications to DB
        
        if (usersToBeNotified.Any())
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationGroupsResult.NotificationId);
            try
            {
                //await _unitOfWork.BeginTransactionAsync();
                foreach (var userId in usersToBeNotified)
                {
                    UserNotification userNotification = new UserNotification
                    {
                        UserId = userId,
                        Notification = notification,
                        ObjectId = ObjectId,
                        ObjectType = (int)objectType,
                        NotificationDate = DateTime.UtcNow,
                        NotificationString = notificationMessage,
                        IsRead = false
                    };
                    await _userNotificationRepository.AddAsync(userNotification);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                //await _unitOfWork.CommitAsync();
            }
            catch
            {
                //await _unitOfWork.RollbackAsync();
                throw;
            }
        }
        return (notificationMessage, usersToBeNotified);
    }


    private async Task<(List<Guid> NotificationGroupsIds,Guid NotificationId)> GetNotificationGroups(string notificationConst)
    {
        var notificationId = await _notificationRepository.GetIdByNameAsync(notificationConst);
        var notificationGroupsIds = await _notificationGroupDetailRepository.GetAll()
                .Where(d => d.NotificationId == notificationId)
                .Select(d => d.NotificationGroupId)
                .ToListAsync();
        return (notificationGroupsIds, notificationId);
    }

    private async Task<List<string>> GetUsersIdsOfNotificationGroups(List<Guid> notificationGroupsIds)
    {
        var usersIds = await _userNotificationGroupRepository.GetAll()
                .Where(uNG => notificationGroupsIds.Contains(uNG.NotificationGroupId))
                .Select(uNG => uNG.UserId)
                .ToListAsync();
        return usersIds;
    }
}
