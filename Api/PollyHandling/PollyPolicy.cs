using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Polly;
using Polly.Retry;

namespace Offers.CleanArchitecture.Api.PollyHandling;

public class PollyPolicy
{
    private readonly ILogger<PollyPolicy> _logger;

    public AsyncRetryPolicy<ObjectResult> RetryObjectResultFife { get; }

    public PollyPolicy(ILogger<PollyPolicy> logger)
    {
        _logger = logger;
        RetryObjectResultFife = Policy.HandleResult<ObjectResult>(
            m => !(m.StatusCode == StatusCodes.Status200OK || m.StatusCode == StatusCodes.Status204NoContent))
            .WaitAndRetryAsync(5, retryAttempt =>
            //TimeSpan.FromSeconds(Math.Pow(2,retryAttempt)));
            TimeSpan.FromSeconds(2));
    }
}
