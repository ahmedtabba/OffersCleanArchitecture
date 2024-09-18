using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices;
public static class InitialiserExtensions
{
    public static async Task InitializeQuartz(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationQuartzInitialiser>();

        await initialiser.Initialize();
    }
}

public class ApplicationQuartzInitialiser
{
    private readonly ILogger<ApplicationQuartzInitialiser> _logger;
    private readonly QuartzConfig _quartzConfig;
    private readonly QuartzJobScheduler _jobScheduler;
    private readonly IConfiguration _configuration;

    public ApplicationQuartzInitialiser(ILogger<ApplicationQuartzInitialiser> logger,
                                        QuartzConfig quartzConfig,
                                        QuartzJobScheduler jobScheduler,
                                        IConfiguration configuration)
    {
        _logger = logger;
        _quartzConfig = quartzConfig;
        _jobScheduler = jobScheduler;
        _configuration = configuration;
    }

    public async Task Initialize()
    {
        // start main scheduler
        await _quartzConfig.Start();

        //get values from appsetting.json
        var numberOfDayBefore = _configuration["ClearUserNotification:NumberOfDayBeforeToCleanUsersNotifications"];
        var clearUserNotificationCronSchedule = _configuration["ClearUserNotification:CronSchedule"];
        var languageAndGlossaryInMemoryCachingCronSchedule = _configuration["languageAndGlossaryInMemoryCaching:CronSchedule"];

        //get tuples of (IJobDetail,ITrigger) that we want to schedule them
        var notificationCleanerTuple = _jobScheduler.ScheduleNotificationCleaner(Convert.ToInt32(numberOfDayBefore), clearUserNotificationCronSchedule);
        var cachingLanguageAndGlossariesTuple = _jobScheduler.ScheduleCachingLanguageAndGlossaries(languageAndGlossaryInMemoryCachingCronSchedule);

        //Schedule the jobs in main scheduler
        await _quartzConfig.Scheduler.ScheduleJob(notificationCleanerTuple.Job, notificationCleanerTuple.Trigger);
        await _quartzConfig.Scheduler.ScheduleJob(cachingLanguageAndGlossariesTuple.Job, cachingLanguageAndGlossariesTuple.Trigger);
        // force quartz to start the job responsible for caching languages and glossaries, then the job will start with cron expression
        await _quartzConfig.Scheduler.TriggerJob(cachingLanguageAndGlossariesTuple.Job.Key);

    }
}
