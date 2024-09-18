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
public class GroceryRepository : BaseRepository<Grocery>, IGroceryRepository
{
    public GroceryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Grocery?> GetGroceryWithPostsByGroceryId(Guid id)
    {
        return await DbContext.Set<Grocery>()
                                .Include(c => c.Posts)
                                .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Grocery?> GetGroceryWithFavoraiteByGroceryId(Guid id)
    {
        return await DbContext.Set<Grocery>()
                                .Include(c => c.FavoraiteGroceries)
                                .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<string> GetGroceryNameByGroceryIdAsync(Guid id)
    {
        var grocery = await DbContext.Groceries
            .FirstOrDefaultAsync(c => c.Id == id);
        if (grocery != null)
            return grocery.Name;
        return string.Empty;
    }

    public IQueryable<Grocery> GetAllByCountryId(Guid countryId)
    {
        return DbContext.Groceries
            .Where(g => g.CountryId == countryId);
    }
}
