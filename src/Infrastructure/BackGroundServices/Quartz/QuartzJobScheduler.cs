using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Offers.CleanArchitecture.Application.Common.Interfaces;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz.Jobs;
using Quartz;
using static Quartz.Logging.OperationName;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz;
public class QuartzJobScheduler // this class contains definitions of what we need to schedule
{                               // every method here return tuple of (IJobDetail,ITrigger) of the job we want to schedule into our scheduler in ApplicationQuartzInitialiser
    public QuartzJobScheduler()
    {
        
    }
    /// <summary>
    /// configure the Notification Cleaner job that will clean the old and read user notifications
    /// </summary>
    /// <param name="numOfDay">the number of days before now to use delete user notifications</param>
    /// <param name="cronSchedule">the time that the trigger will be fired in Cron string</param>
    /// <returns>tuple of (IJobDetail,ITrigger) to be scheduled in main scheduler</returns>
    public (IJobDetail Job,ITrigger Trigger) ScheduleNotificationCleaner(int numOfDay, string cronSchedule)
    {

        //NotificationCleanerJob is the IJob class responsible for execute the job
        var jobKey = JobKey.Create(nameof(NotificationCleanerJob));
        IJobDetail job = JobBuilder.Create<NotificationCleanerJob>()
            .WithIdentity(jobKey)
            .UsingJobData("numberOfDayBefore", numOfDay)// passing numOfDay to NotificationCleanerJob
            .Build();

        //ITrigger trigger = TriggerBuilder.Create()
        //.WithIdentity("NotificationCleanerTrigger")
        //.WithCronSchedule(cronSchedule)
        //.Build();

        // this trigger is for test and we will use trigger with CronSchedule later
        ITrigger trigger2 = TriggerBuilder.Create()
        .WithIdentity("NotificationCleanerTrigger2")
        .StartNow()
        .Build();
        return(job, trigger2);

    }

    public (IJobDetail Job, ITrigger Trigger) ScheduleCachingGroceries(Guid countryId)
    {

        //CachingGroceriesJob is the IJob class responsible for execute the job
        // every country will have a job to save its groceries after the first request, so we will have a number of kay-value in memory represent that equal to countries number
        var jobKey = JobKey.Create(nameof(CachingGroceriesJob) + "-" + countryId.ToString());
        IJobDetail job = JobBuilder.Create<CachingGroceriesJob>()
            .WithIdentity(jobKey)
            .UsingJobData("country-id", countryId)// passing country-id to CachingGroceriesJob
            .Build();

        // this trigger will be fired one time after 30 seconds
        var triggerKey = "CachingGroceriesTrigger" + "-" + countryId.ToString();
        ITrigger trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(DateTimeOffset.Now.AddSeconds(30))
        .Build();
        return (job, trigger);

    }


    public (IJobDetail Job, ITrigger Trigger) ScheduleCachingGroceriesLocalization(Guid groceryId)
    {

        //CachingGroceriesLocalizationJob is the IJob class responsible for execute the job
        // every grocery will have a job to save its localization after the first request,
        // so we will have a number of kay-value in memory represent localization of each grocery equal to grocery number
        var jobKey = JobKey.Create(nameof(CachingGroceriesLocalizationJob) + "-" + groceryId.ToString());
        IJobDetail job = JobBuilder.Create<CachingGroceriesLocalizationJob>()
            .WithIdentity(jobKey)
            .UsingJobData("grocery-id", groceryId)// passing grocery-id to CachingGroceriesJob
            .Build();

        // this trigger will be fired one time after 35 seconds
        var triggerKey = "CachingGroceriesLocalizationTrigger" + "-" + groceryId.ToString();

        ITrigger trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .StartAt(DateTimeOffset.Now.AddSeconds(35))
        .Build();
        return (job, trigger);

    }

    public (IJobDetail Job, ITrigger Trigger) ScheduleCachingLanguageAndGlossaries(string cronSchedule)
    {

        
        var jobKey = JobKey.Create(nameof(CachingGlossariesJob));
        IJobDetail job = JobBuilder.Create<CachingGlossariesJob>()
            .WithIdentity(jobKey)
            .Build();


        var triggerKey = new TriggerKey("CachingGlossariesTrigger");

        ITrigger trigger = TriggerBuilder.Create()
        .WithIdentity(triggerKey)
        .WithCronSchedule(cronSchedule)
        .Build();
        return (job, trigger);

    }
}
