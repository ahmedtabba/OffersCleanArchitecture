using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Domain.Entities;
using static Offers.CleanArchitecture.Infrastructure.Utilities.RoleConsistent;

namespace Offers.CleanArchitecture.Infrastructure.Repositories.CachedRepositories;
public class CachedGroceryLocalizationRepository : IGroceryLocalizationRepository
{
    private readonly IGroceryLocalizationRepository _decorated;// the main repository that we decorate it
    private readonly ICacheService _cacheService;
    private readonly ISeedJobs _seedJobs;

    public CachedGroceryLocalizationRepository(IGroceryLocalizationRepository groceryLocalizationRepository,
                                               ICacheService cacheService,
                                               ISeedJobs seedJobs)
    {
        _decorated = groceryLocalizationRepository;
        _cacheService = cacheService;
        _seedJobs = seedJobs;
    }
    public Task<GroceryLocalization> AddAsync(GroceryLocalization entity) => _decorated.AddAsync(entity);


    public Task DeleteAsync(GroceryLocalization entity) => _decorated.DeleteAsync(entity);
   

    public IQueryable<GroceryLocalization> GetAll() => _decorated.GetAll();
    

    public IQueryable<GroceryLocalization> GetAllAsTracking() => _decorated.GetAllAsTracking();


    public Task<GroceryLocalization> GetByIdAsync(Guid id) => _decorated.GetByIdAsync(id);
   

    public Task UpdateAsync(GroceryLocalization entity) => _decorated.UpdateAsync(entity);

    // the method that we will cache the data in its implementation
    public async Task<List<GroceryLocalization>> GetAllByGroceryId(Guid groceryId)
    {
        // the key that we will use it to save and retrieve the cached data from memory, every grocery will have variable in memory represent the localization of it after the first request to get it
        var key = "groceryLocalization-" + groceryId.ToString();
        // check if we have the data in memory using the Key of the GroceryLocalization
        var groceriesLocalizationCached = _cacheService.GetData<IEnumerable<GroceryLocalization>>(key);
        if (groceriesLocalizationCached == null)
        {
            // the case of no data in memory: get the data from Db and seed job to cache it in memory
            var groceriesLocalization = await _decorated.GetAllByGroceryId(groceryId);
            // seed job to cache groceriesLocalization of the grocery requested
            _seedJobs.CacheGroceriesLocalization(groceryId).Wait();
            return groceriesLocalization;
        }
        else
        {
            // the case of existing data of the Key of the GroceryLocalization
            var cachedGroceriesLocalization = _cacheService.GetData<IEnumerable<GroceryLocalization>>(key);
            return cachedGroceriesLocalization.ToList();
        }
    }
}
