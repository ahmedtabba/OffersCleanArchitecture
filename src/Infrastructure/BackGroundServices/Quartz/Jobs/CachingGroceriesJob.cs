using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Services;
using Quartz;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz.Jobs;
public class CachingGroceriesJob : IJob // this job responsible for caching groceries of country in memory
{
    private readonly ILogger<CachingGroceriesJob> _logger;
    private readonly ICacheService _cacheService;
    private readonly IGroceryRepository _groceryRepository;

    public CachingGroceriesJob(ILogger<CachingGroceriesJob> logger,
                               ICacheService cacheService,
                               IGroceryRepository groceryRepository)
    {
        _logger = logger;
        _cacheService = cacheService;
        _groceryRepository = groceryRepository;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        // retrieve countryId from QuartzJobScheduler service
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        var isThereValue = dataMap.TryGetGuidValue("country-id", out Guid temp);
        if (isThereValue)
        {
            // check if we have data in memory for groceries of the country, if yes, delete it and save new one
            var key = "groceries-" + temp.ToString();
            var value = _cacheService.GetData<IEnumerable<Grocery>>(key);
            if (value != null)
            {
                _cacheService.RemoveData(key);
            }
            // get data from Db
            var groceries = await _groceryRepository.GetAll()
                .Where(g => g.CountryId == temp)
                .ToListAsync();
            IEnumerable<Grocery> result = groceries;
            // save data in memory for 1 day
            _cacheService.SetData<IEnumerable<Grocery>>(key, result, DateTimeOffset.Now.AddDays(1));
        };
    }
}
