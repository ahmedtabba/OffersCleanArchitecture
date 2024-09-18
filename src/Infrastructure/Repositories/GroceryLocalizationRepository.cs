using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class GroceryLocalizationRepository : BaseRepository<GroceryLocalization>, IGroceryLocalizationRepository
{
    public GroceryLocalizationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<GroceryLocalization>> GetAllByGroceryId(Guid groceryId)
    {
        return await DbContext.GroceryLocalization
            .Where(gl => gl.GroceryId == groceryId)
            .ToListAsync();
    }
}
