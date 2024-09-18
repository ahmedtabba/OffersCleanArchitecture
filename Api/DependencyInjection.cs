using Offers.CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Offers.CleanArchitecture.Api.Infrastructure;
using Offers.CleanArchitecture.Api.Services;
using Offers.CleanArchitecture.Infrastructure.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Polly;
using Polly.Timeout;
using Polly.Registry;
using Offers.CleanArchitecture.Api.PollyHandling;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;










#if (UseApiOnly)
using NSwag;
using NSwag.Generation.Processors.Security;
#endif
using ZymLabs.NSwag.FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
        

        services.AddScoped<IUser, CurrentUser>();
        services.AddScoped<IApplicationUserGroup, ApplicationUserGroup>();// الاغلب لا داعي لها واضفناها اول عملنا في كونترولر الايدينتتي

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddRazorPages();

        services.AddScoped(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        //services.AddSwaggerGen(c =>
        //{
        //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //    {
        //        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
        //              Enter 'Bearer' [space] and then your token in the text input below.
        //              \r\n\r\nExample: 'Bearer 12345abcdef'",
        //        Name = "Authorization",
        //        In = ParameterLocation.Header,
        //        Type = SecuritySchemeType.ApiKey,
        //        Scheme = "Bearer"
        //    });

        //    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        //          {
        //            {
        //              new OpenApiSecurityScheme
        //              {
        //                Reference = new OpenApiReference
        //                  {
        //                    Type = ReferenceType.SecurityScheme,
        //                    Id = "Bearer"
        //                  },
        //                  Scheme = "oauth2",
        //                  Name = "Bearer",
        //                  In = ParameterLocation.Header,

        //                },
        //                new List<string>()
        //              }
        //            });

        //    c.SwaggerDoc("v1", new OpenApiInfo
        //    {
        //        Version = "v1",
        //        Title = "Clean Architecture Management API",

        //    });
        //});

        services.AddSingleton<PollyPolicy>(x =>
                                            new PollyPolicy(
                                                x.GetRequiredService<ILogger<PollyPolicy>>()
                                                )
                                            );
        // Register ResiliencePipeline to inject it in controllers
        /*
        services.AddResiliencePipeline<string, HttpResponseMessage>("retry-pipe", builder =>
        {
            builder.AddRetry(new()
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    //.Handle<HttpRequestException>()
                    .HandleResult(response => response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            });
        });
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        ResiliencePipelineProvider<string> pipelineProvider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();
        ResiliencePipeline<HttpResponseMessage> pipeline = pipelineProvider.GetPipeline<HttpResponseMessage>("retry-pipe");
        */
        return services;
    }

    
}
