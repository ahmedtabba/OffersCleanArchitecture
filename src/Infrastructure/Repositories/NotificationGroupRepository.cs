using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class NotificationGroupRepository : BaseRepository<NotificationGroup>, INotificationGroupRepository
{
    public NotificationGroupRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<NotificationGroup> GetWithNotificationsByIdAsync(Guid notificationGroupId)
    {
        return await DbContext.NotificationGroups
            .Include(n => n.Notifications)
            .FirstOrDefaultAsync(n => n.Id == notificationGroupId);
    }
}
