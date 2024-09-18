using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Quartz;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz.Jobs;
public class NotificationCleanerJob : IJob // this responsible for connecting to DB and clean user notifications
{
    private readonly ILogger<NotificationCleanerJob> _logger;
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private int NumberOfDayBefore = 30;// default value is one month
    public NotificationCleanerJob(ILogger<NotificationCleanerJob> logger,
                                  IUserNotificationRepository userNotificationRepository,
                                  IUnitOfWorkAsync unitOfWork)
    {
        _logger = logger;
        _userNotificationRepository = userNotificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            // dataMap is dictionary has values passed from IJobDetail witch joined to this IJob
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var isThereValue = dataMap.TryGetIntValue("numberOfDayBefore", out int temp);
            if (isThereValue)
            {
                NumberOfDayBefore = temp;
            }
            var notifications = await _userNotificationRepository.GetAllAsTracking()
                .Where(n => n.NotificationDate < DateTime.UtcNow.AddDays(-NumberOfDayBefore) && n.IsRead)
                .ToListAsync();
            _userNotificationRepository.DeleteRange(notifications);
            CancellationToken c = default;
            await _unitOfWork.SaveChangesAsync(c);
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }

    }
}
