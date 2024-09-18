using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Offers.CleanArchitecture.Application.Common.Interfaces.Assets;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Domain.Constants;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices;
using Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz;
using Offers.CleanArchitecture.Infrastructure.Data;
using Offers.CleanArchitecture.Infrastructure.Data.Interceptors;
using Offers.CleanArchitecture.Infrastructure.Hubs;
using Offers.CleanArchitecture.Infrastructure.Identity;
using Offers.CleanArchitecture.Infrastructure.Repositories;
using Offers.CleanArchitecture.Infrastructure.Repositories.Assets;
using Offers.CleanArchitecture.Infrastructure.Repositories.CachedRepositories;
using Offers.CleanArchitecture.Infrastructure.Services;
using Quartz;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

#if (UseSQLite)
            options.UseSqlite(connectionString);
#else
            options.UseLazyLoadingProxies(false);
            options.UseSqlServer(connectionString);
     
#endif
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddSingleton<ApplicationQuartzInitialiser>();
        services.AddSingleton<QuartzJobScheduler>();
        services.AddSingleton<QuartzConfig>();

        services.AddMemoryCache();

        services.AddScoped(typeof(IRepositoryAsync<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWorkAsync,UnitOfWorkAsync>();
        services.AddScoped<IGroceryRepository, GroceryRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IFavoraiteGroceryRepository, FavoraiteGroceryRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ILanguageRepository, LanguageRepository>();
        services.AddScoped<IGroceryLocalizationRepository, GroceryLocalizationRepository>();
        services.AddScoped<IPostLocalizationRepository, PostLocalizationRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationGroupRepository, NotificationGroupRepository>();
        services.AddScoped<INotificationGroupDetailRepository, NotificationGroupDetailRepository>();
        services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
        services.AddScoped<IUserNotificationGroupRepository, UserNotificationGroupRepository>();
        services.AddScoped<IOnboardingPageRepository, OnboardingPageRepository>();
        services.AddScoped<IOnboardingPageLocalizationRepository, OnboardingPageLocalizationRepository>();
        services.AddScoped<IGlossaryRepository, GlossaryRepository>();
        services.AddScoped<IGlossaryLocalizationRepository, GlossaryLocalizationRepository>();

        services.Decorate<IGroceryRepository, CachedGroceryRepository>();
        services.Decorate<IGroceryLocalizationRepository, CachedGroceryLocalizationRepository>();
        services.Decorate<ILanguageRepository, CachedLanguageRepository>();
        services.Decorate<IGlossaryLocalizationRepository, CachedGlossaryLocalizationRepository>();
        services.Decorate<IGlossaryRepository, CachedGlossaryRepository>();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ISeedJobs, SeedJobs>();

        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserNotificationService, UserNotificationService>();

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IFileService, LocalFileService>();

        //services.AddSingleton<ISchedulerFactory>();
        //services.AddScoped<GroupStoreBase>();
        //services.AddScoped(typeof(IApplicationUserGroup), typeof(ApplicationUserGroup));

#if (UseApiOnly)
        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
#else
        //services
        //    .AddDefaultIdentity<ApplicationUser>()
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<ApplicationDbContext>();
#endif

        services.AddSingleton(TimeProvider.System);
        services.AddQuartz(o =>
        {
            o.UseMicrosoftDependencyInjectionJobFactory();
        }

        );
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);




        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddDbContext<CleanArchitectureIdentityDbContext>(options => 
        {
            options.UseSqlServer(configuration.GetConnectionString("CleanArchitectureIdentityConnectionString"),
                                  b => b.MigrationsAssembly(typeof(CleanArchitectureIdentityDbContext).Assembly.FullName));
            options.UseLazyLoadingProxies(false);
        });





        services.AddIdentity<ApplicationUser, ApplicationRole>(opt => // TODO replace IdentityRole => ApplicationRole
        {
            opt.Password.RequireUppercase = true;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireNonAlphanumeric = true;
            opt.Password.RequiredLength = 8;
        })
            .AddEntityFrameworkStores<CleanArchitectureIdentityDbContext>().AddDefaultTokenProviders();

        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<PasswordValidator<ApplicationUser>>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                };
                o.Authority = "GloboTicketIdentity";
                o.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = c =>
                    {
                        var accessToken = c.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = c.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/notificationHub")))
                        {
                            // Read the token out of the query string
                            c.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
                //o.Events = new JwtBearerEvents()
                //{
                //    OnAuthenticationFailed = c =>
                //    {
                //        c.NoResult();
                //        c.Response.StatusCode = 500;
                //        c.Response.ContentType = "text/plain";
                //        return c.Response.WriteAsync(c.Exception.ToString());
                //    },
                //    OnChallenge = context =>
                //    {
                //        context.HandleResponse();
                //        context.Response.StatusCode = 401;
                //        context.Response.ContentType = "application/json";
                //        var result = JsonSerializer.Serialize("401 Not authorized");
                //        return context.Response.WriteAsync(result);
                //    },
                //    OnForbidden = context =>
                //    {
                //        context.Response.StatusCode = 403;
                //        context.Response.ContentType = "application/json";
                //        var result = JsonSerializer.Serialize("403 Not authorized");
                //        return context.Response.WriteAsync(result);
                //    }
                //};
            });

        services.AddTransient<IIdentityService, IdentityService>();
        //services.AddTransient<IApplicationGroupManager, ApplicationGroupManager>();
        services.AddTransient<IApplicationGroupManager>(x =>
                                                        new ApplicationGroupManager(
                                                         x.GetRequiredService<CleanArchitectureIdentityDbContext>(),
                                                         x.GetRequiredService<UserManager<ApplicationUser>>(),
                                                         x.GetRequiredService<RoleManager<ApplicationRole>>())
                                                        );
        services.AddTransient<IApplicationPasswordValidator>(x=>
                                                             new ApplicationPasswordValidator(
                                                             x.GetRequiredService<UserManager<ApplicationUser>>(),
                                                             x.GetRequiredService<PasswordValidator<ApplicationUser>>()));


        services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        return services;
    }
}
