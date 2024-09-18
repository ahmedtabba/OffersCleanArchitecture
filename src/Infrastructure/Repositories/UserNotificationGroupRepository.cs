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
public class UserNotificationGroupRepository : BaseRepository<UserNotificationGroup>, IUserNotificationGroupRepository
{
    public UserNotificationGroupRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
    
}
