using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Repositories;
using Quartz;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz.Jobs;
public class CachingGroceriesLocalizationJob : IJob // this job responsible for caching grocery localization of grocery in memory
{
    private readonly ILogger<CachingGroceriesLocalizationJob> _logger;
    private readonly ICacheService _cacheService;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;

    public CachingGroceriesLocalizationJob(ILogger<CachingGroceriesLocalizationJob> logger,
                                           ICacheService cacheService,
                                           IGroceryLocalizationRepository groceryLocalizationRepository)
    {
        _logger = logger;
        _cacheService = cacheService;
        _groceryLocalizationRepository = groceryLocalizationRepository;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        // retrieve countryId from QuartzJobScheduler service
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        var isThereValue = dataMap.TryGetGuidValue("grocery-id", out Guid temp);
        if (isThereValue)
        {
            // check if we have data in memory for localization of the grocery, if yes, delete it and save new one
            var key = "groceryLocalization-" + temp.ToString();
            var value = _cacheService.GetData<IEnumerable<GroceryLocalization>>(key);
            if (value != null)
            {
                _cacheService.RemoveData(key);
            }
            // get data from Db
            var groceriesLocalization = await _groceryLocalizationRepository.GetAll()
                .Where(gl => gl.GroceryId == temp)
            .ToListAsync();
            IEnumerable<GroceryLocalization> result = groceriesLocalization;
            // save data in memory for 1 day
            _cacheService.SetData<IEnumerable<GroceryLocalization>>(key, result, DateTimeOffset.Now.AddDays(1));
        }
        

    }
}
