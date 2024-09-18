using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz.Jobs;
using Quartz;

namespace Offers.CleanArchitecture.Infrastructure.Services;
public class SeedJobs : ISeedJobs
{
    private readonly ILogger<SeedJobs> _logger;
    private readonly QuartzConfig _quartzConfig;
    private readonly QuartzJobScheduler _quartzJobScheduler;

    public SeedJobs(ILogger<SeedJobs> logger,
                    QuartzConfig quartzConfig,
                    QuartzJobScheduler quartzJobScheduler)
    {
        _logger = logger;
        _quartzConfig = quartzConfig;
        _quartzJobScheduler = quartzJobScheduler;
    }
    // implementation of add job to cache localization of grocery 
    public async Task CacheGroceriesLocalization(Guid groceryId)
    {
        //get job -will cache localization of grocery- and its trigger to schedule the job in main Scheduler
        var cacheGroceriesLocalizationTuple = _quartzJobScheduler.ScheduleCachingGroceriesLocalization(groceryId);

        // check if same job seeded again before executing the job
        var jobKey = JobKey.Create(nameof(CachingGroceriesLocalizationJob) + "-" + groceryId.ToString());
        var isThereJob = await _quartzConfig.Scheduler.GetJobDetail(jobKey);
        // if no job -as the same as the job we will schedule it- has seeded,schedule the job, otherwise, ignore
        if (isThereJob == null)
        {
            await _quartzConfig.Scheduler.ScheduleJob(cacheGroceriesLocalizationTuple.Job, cacheGroceriesLocalizationTuple.Trigger);
        }
    }

    // implementation of add job to cache localization of grocery 
    public async Task CacheGroceriesOfCountry(Guid countryId)
    {
        //get job -will cache groceries of country- and its trigger to schedule the job in main Scheduler
        var cacheGroceriesTuple = _quartzJobScheduler.ScheduleCachingGroceries(countryId);

        // check if same job seeded again before executing the job
        var jobKey = JobKey.Create(nameof(CachingGroceriesJob) + "-" + countryId.ToString());
        var isThereJob =  await _quartzConfig.Scheduler.GetJobDetail(jobKey);
        // if no job -as the same as the job we will schedule it- has seeded,schedule the job, otherwise, ignore
        if (isThereJob == null)
        {
            await _quartzConfig.Scheduler.ScheduleJob(cacheGroceriesTuple.Job, cacheGroceriesTuple.Trigger);
        }
        
    }
}
