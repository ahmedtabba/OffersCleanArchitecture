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
public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Guid> GetIdByNameAsync(string notificationName)
    {
        var notification = await DbContext.Notifications
            .SingleOrDefaultAsync(n => n.Name == notificationName);
        return notification.Id;
    }
}
