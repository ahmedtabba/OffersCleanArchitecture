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
public class FavoraiteGroceryRepository : BaseRepository<FavoraiteGrocery>, IFavoraiteGroceryRepository
{
    public FavoraiteGroceryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<FavoraiteGrocery>>? GetFavoraiteGroceriesWithGroceriesByUserId(string userId)
    {
        return await DbContext.Set<FavoraiteGrocery>()
                               .Include(fg => fg.Grocery)
                               .Where(fg => fg.UserId == userId)
                               .ToListAsync();
    }

    public  IQueryable<FavoraiteGrocery>? GetFavoraiteGroceriesWithGroceriesBy_UserId(string userId)
    {
        return  DbContext.Set<FavoraiteGrocery>()
                               .Include(fg => fg.Grocery)
                               .Where(fg => fg.UserId == userId)
                               .AsQueryable();
    }
    public async Task<bool> CheckIfUserHasFavorate(string userId)
    {
        return await DbContext.Set<FavoraiteGrocery>()
                               .AnyAsync(fg => fg.UserId == userId);
    }
    public async Task<FavoraiteGrocery>? GetFavoraiteGroceryWithGroceriesByFavoraiteGroceryId(Guid id)
    {
        return await DbContext.Set<FavoraiteGrocery>()
                                .Include(fg => fg.Grocery)
                                .FirstOrDefaultAsync(fg => fg.Id ==id);
    }

}
