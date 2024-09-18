using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces;
using Quartz;
using Quartz.Impl;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices;
public class QuartzConfig // this class responsible for register scheduler and its configuration and start,stop it
{
    private readonly ILogger<QuartzConfig> _logger;
    private readonly ISchedulerFactory _schedulerFactory;
    public IScheduler Scheduler { get; } 
    public QuartzConfig(ILogger<QuartzConfig> logger,
                        ISchedulerFactory schedulerFactory)
    {
        _logger = logger;
        _schedulerFactory = schedulerFactory;
        Scheduler = _schedulerFactory.GetScheduler().Result;
    }
    public async Task Start()
    {
        // configuration of Scheduler...
        await Scheduler.Start();
    }
    public async Task Stop()
    {
        //logic
        await Scheduler.Shutdown();
    }


}
