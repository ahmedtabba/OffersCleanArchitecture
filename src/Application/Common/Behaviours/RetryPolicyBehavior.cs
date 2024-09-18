using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.PollyAttributes;
using Polly;

namespace Offers.CleanArchitecture.Application.Common.Behaviours;
public class RetryPolicyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest:notnull
{
    private readonly ILogger<RetryPolicyBehavior<TRequest, TResponse>> _logger;

    public RetryPolicyBehavior(ILogger<RetryPolicyBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var retryAttr = typeof(TRequest).GetCustomAttribute<RetryPolicyAttribute>();
        if (retryAttr == null)
        {
            return await next();
        }
        return await Policy.Handle<Exception>()
        .WaitAndRetryAsync(retryAttr.RetryCount,
        i => TimeSpan.FromMilliseconds(i * retryAttr.SleepDuration),
        (ex, ts, _) => _logger.LogWarning(ex, "Failed to execute handler for request {Request}, retrying after {RetryTimeSpan}s: {ExceptionMessage}",
                                          typeof(TRequest).Name, ts.TotalSeconds, ex.Message))
        .ExecuteAsync(async () => await next());
   
    }
}
