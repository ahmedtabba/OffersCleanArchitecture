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
public class UserNotificationRepository : BaseRepository<UserNotification>, IUserNotificationRepository
{
    public UserNotificationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public void DeleteRange(List<UserNotification> notifications)
    {
         DbContext.UserNotifications.RemoveRange(notifications);
   
    }
}
