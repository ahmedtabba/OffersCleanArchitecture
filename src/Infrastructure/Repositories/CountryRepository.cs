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
public class CountryRepository : BaseRepository<Country>, ICountryRepository
{
    public CountryRepository(AppDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<string> GetCountryNameByIdAsync(Guid countryId)
    {
        var country = await DbContext.Countries
            .FirstOrDefaultAsync(c => c.Id == countryId);
        if (country != null)
            return country.Name;
        else
            return string.Empty;
    }

    public async Task<Country> GetCountryWithGroceriesByCountryIdAsync(Guid countryId)
    {
        var country = await DbContext.Countries
            .Include(c => c.Groceries)
            .FirstOrDefaultAsync(c => c.Id == countryId);
        return country;
    }
}
