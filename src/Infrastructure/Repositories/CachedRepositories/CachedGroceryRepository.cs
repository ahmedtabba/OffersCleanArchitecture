using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Repositories.CachedRepositories;
public class CachedGroceryRepository :  IGroceryRepository
{
    private readonly IGroceryRepository _decorated;// the main repository that we decorate it
    private readonly ICacheService _cacheService;
    private readonly ISeedJobs _seedJobs;

    public CachedGroceryRepository(IGroceryRepository groceryRepository,
                                   ICacheService cacheService,
                                   ISeedJobs seedJobs)
    {
        _decorated = groceryRepository;
        _cacheService = cacheService;
        _seedJobs = seedJobs;
    }
    public Task<Grocery> AddAsync(Grocery entity) => _decorated.AddAsync(entity);
 

    public Task DeleteAsync(Grocery entity) => _decorated.DeleteAsync(entity);
    

    public IQueryable<Grocery> GetAll() => _decorated.GetAll();
 

    public IQueryable<Grocery> GetAllAsTracking() => _decorated.GetAllAsTracking();

    // the method that we will cache the data in its implementation
    public IQueryable<Grocery> GetAllByCountryId(Guid countryId)
    {
        // the key that we will use it to save and retrieve the cached data from memory, every country will have variable in memory represent the groceries of it after the first request to get it
        var key = "groceries-" + countryId.ToString();
        // check if we have the data in memory using the Key of the country's groceries
        var groceriesCached = _cacheService.GetData<IEnumerable<Grocery>>(key);
        if (groceriesCached == null)
        {
            // the case of no data in memory: get the data from Db and seed job to cache it in memory
            var groceries = _decorated.GetAllByCountryId(countryId);
            // seed job to cache groceries of country requested
            _seedJobs.CacheGroceriesOfCountry(countryId).Wait();
            return groceries;
        }
        else
        {
            // the case of existing data of the Key 'groceries-countryId'
            IEnumerable<Grocery> cachedGroceries = _cacheService.GetData<IEnumerable<Grocery>>(key);
            // here we return the result as Queryable because we will paginate it
            IQueryable<Grocery> groceries = cachedGroceries.AsQueryable();
            return groceries;
        }
    }

    public Task<Grocery> GetByIdAsync(Guid id) => _decorated.GetByIdAsync(id);
   

    public Task<string> GetGroceryNameByGroceryIdAsync(Guid id) => _decorated.GetGroceryNameByGroceryIdAsync(id);
   

    public Task<Grocery?> GetGroceryWithFavoraiteByGroceryId(Guid id) => _decorated.GetGroceryWithFavoraiteByGroceryId(id);


    public Task<Grocery?> GetGroceryWithPostsByGroceryId(Guid id) => _decorated.GetGroceryWithPostsByGroceryId(id);
    

    public Task UpdateAsync(Grocery entity) => _decorated.UpdateAsync(entity);
   
}
