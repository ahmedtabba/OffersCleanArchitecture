using System.Runtime.InteropServices;
using Offers.CleanArchitecture.Domain.Constants;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Polly;
using System.Data;
using Offers.CleanArchitecture.Infrastructure.Utilities;
using Offers.CleanArchitecture.Application.Utilities;
using Microsoft.Extensions.Configuration;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly AppDbContext _context;
    private readonly CleanArchitectureIdentityDbContext _identityContext;
    private readonly IApplicationGroupManager _applicationGroupManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
                                           AppDbContext context,
                                           UserManager<ApplicationUser> userManager,
                                           RoleManager<ApplicationRole> roleManager,
                                           CleanArchitectureIdentityDbContext identityContext,
                                           IApplicationGroupManager applicationGroupManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _identityContext = identityContext;
        _applicationGroupManager = applicationGroupManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            await _identityContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }


    //Review:ReSeeding Roles From RoleConstants
    public async Task TrySeedAsync()
    {
        // Default roles
        var roles = _identityContext.Roles.ToListAsync().Result;

        #region Grocery
        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.Browse))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.Browse, NormalizedName = RoleConsistent.Grocery.Browse.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.Add, NormalizedName = RoleConsistent.Grocery.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.Edit, NormalizedName = RoleConsistent.Grocery.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.Delete, NormalizedName = RoleConsistent.Grocery.Delete.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.AddToFavorite))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.AddToFavorite, NormalizedName = RoleConsistent.Grocery.AddToFavorite.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.RemoveFromFavorite))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.RemoveFromFavorite, NormalizedName = RoleConsistent.Grocery.RemoveFromFavorite.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Grocery.BrowseFavorite))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Grocery.BrowseFavorite, NormalizedName = RoleConsistent.Grocery.BrowseFavorite.ToUpper() });
        #endregion

        #region Country
        if (!roles.Exists(r => r.Name == RoleConsistent.Country.BrowseTimeZones))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Country.BrowseTimeZones, NormalizedName = RoleConsistent.Country.BrowseTimeZones.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Country.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Country.Add, NormalizedName = RoleConsistent.Country.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Country.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Country.Edit, NormalizedName = RoleConsistent.Country.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Country.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Country.Delete, NormalizedName = RoleConsistent.Country.Delete.ToUpper() });

        #endregion

        #region Identity
        if (!roles.Exists(r => r.Name == RoleConsistent.Identity.BrowseUsers))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Identity.BrowseUsers, NormalizedName = RoleConsistent.Identity.BrowseUsers.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Identity.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Identity.Add, NormalizedName = RoleConsistent.Identity.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Identity.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Identity.Edit, NormalizedName = RoleConsistent.Identity.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Identity.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Identity.Delete, NormalizedName = RoleConsistent.Identity.Delete.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Identity.ResetMyPassword))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Identity.ResetMyPassword, NormalizedName = RoleConsistent.Identity.ResetMyPassword.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Identity.ResetPasswordByAdmin))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Identity.ResetPasswordByAdmin, NormalizedName = RoleConsistent.Identity.ResetPasswordByAdmin.ToUpper() });
        #endregion

        #region Language
        if (!roles.Exists(r => r.Name == RoleConsistent.Language.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Language.Add, NormalizedName = RoleConsistent.Language.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Language.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Language.Edit, NormalizedName = RoleConsistent.Language.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Language.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Language.Delete, NormalizedName = RoleConsistent.Language.Delete.ToUpper() });

        #endregion

        #region Post
        if (!roles.Exists(r => r.Name == RoleConsistent.Post.BrowseFavoritePosts))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Post.BrowseFavoritePosts, NormalizedName = RoleConsistent.Post.BrowseFavoritePosts.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Post.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Post.Add, NormalizedName = RoleConsistent.Post.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Post.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Post.Edit, NormalizedName = RoleConsistent.Post.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Post.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Post.Delete, NormalizedName = RoleConsistent.Post.Delete.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Post.BrowseForAdmin))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Post.BrowseForAdmin, NormalizedName = RoleConsistent.Post.BrowseForAdmin.ToUpper() });
        #endregion

        #region Glossary
        if (!roles.Exists(r => r.Name == RoleConsistent.Glossary.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Glossary.Edit, NormalizedName = RoleConsistent.Glossary.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.Glossary.BrowseGlossariesForAdmin))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.Glossary.BrowseGlossariesForAdmin, NormalizedName = RoleConsistent.Glossary.BrowseGlossariesForAdmin.ToUpper() });
        #endregion

        #region NotificationGroup
        if (!roles.Exists(r => r.Name == RoleConsistent.NotificationGroup.Browse))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.NotificationGroup.Browse, NormalizedName = RoleConsistent.NotificationGroup.Browse.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.NotificationGroup.BrowseNotifications))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.NotificationGroup.BrowseNotifications, NormalizedName = RoleConsistent.NotificationGroup.BrowseNotifications.ToUpper() });
        
        if (!roles.Exists(r => r.Name == RoleConsistent.NotificationGroup.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.NotificationGroup.Add, NormalizedName = RoleConsistent.NotificationGroup.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.NotificationGroup.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.NotificationGroup.Edit, NormalizedName = RoleConsistent.NotificationGroup.Edit.ToUpper() });
        
        if (!roles.Exists(r => r.Name == RoleConsistent.NotificationGroup.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.NotificationGroup.Delete, NormalizedName = RoleConsistent.NotificationGroup.Delete.ToUpper() });
        #endregion

        #region OnboardingPage
        if (!roles.Exists(r => r.Name == RoleConsistent.OnboardingPage.BrowseOnboardingPageWithLocalization))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.OnboardingPage.BrowseOnboardingPageWithLocalization, NormalizedName = RoleConsistent.OnboardingPage.BrowseOnboardingPageWithLocalization.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.OnboardingPage.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.OnboardingPage.Add, NormalizedName = RoleConsistent.OnboardingPage.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.OnboardingPage.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.OnboardingPage.Edit, NormalizedName = RoleConsistent.OnboardingPage.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.OnboardingPage.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.OnboardingPage.Delete, NormalizedName = RoleConsistent.OnboardingPage.Delete.ToUpper() });
        #endregion

        #region UserGroup
        if (!roles.Exists(r => r.Name == RoleConsistent.UserGroup.BrowseUserGroups))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserGroup.BrowseUserGroups, NormalizedName = RoleConsistent.UserGroup.BrowseUserGroups.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.UserGroup.Add))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserGroup.Add, NormalizedName = RoleConsistent.UserGroup.Add.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.UserGroup.Edit))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserGroup.Edit, NormalizedName = RoleConsistent.UserGroup.Edit.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.UserGroup.Delete))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserGroup.Delete, NormalizedName = RoleConsistent.UserGroup.Delete.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.UserGroup.BrowseRoles))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserGroup.BrowseRoles, NormalizedName = RoleConsistent.UserGroup.BrowseRoles.ToUpper() });
        #endregion

        #region UserNotification
        if (!roles.Exists(r => r.Name == RoleConsistent.UserNotification.Browse))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserNotification.Browse, NormalizedName = RoleConsistent.UserNotification.Browse.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.UserNotification.MakeAsRead))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserNotification.MakeAsRead, NormalizedName = RoleConsistent.UserNotification.MakeAsRead.ToUpper() });

        if (!roles.Exists(r => r.Name == RoleConsistent.UserNotification.MakeAsUnRead))
            _identityContext.Roles.Add(new ApplicationRole { Name = RoleConsistent.UserNotification.MakeAsUnRead, NormalizedName = RoleConsistent.UserNotification.MakeAsUnRead.ToUpper() });

        #endregion

        await _identityContext.SaveChangesAsync();
        //check all roles to remove the already saved roles but they aren't existed in RoleConsistent
        var rolesAfterInitialize = _identityContext.Roles.ToListAsync().Result;
        // we make separated lists for each class in RoleConsistent to do checking and avoid nested if statements
        var groceryRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Grocery\"));
        var countryRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Country\"));
        var identityRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Identity\"));
        var languageRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Language\"));
        var postRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Post\"));
        var glossaryRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Glossary\"));
        var notificationGroupRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Notification Group\"));
        var onboardingPageRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"Onboarding Page\"));
        var userGroupRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"User Group\"));
        var userNotificationRoles = rolesAfterInitialize.Where(r => r.Name.StartsWith(@"User Notification\"));
        //clear rolesAfterInitialize from all roles that we will check, so if it is not empty now, we will remove all remaining role that not associated to RoleConsistent
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(groceryRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(countryRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(identityRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(languageRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(postRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(glossaryRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(notificationGroupRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(onboardingPageRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(userGroupRoles).ToList();
        rolesAfterInitialize = rolesAfterInitialize.Except<IdentityRole>(userNotificationRoles).ToList();

        foreach (var role in groceryRoles)
        {
            if (!new RoleConsistent.Grocery().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in countryRoles)
        {
            if (!new RoleConsistent.Country().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in identityRoles)
        {
            if (!new RoleConsistent.Identity().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in languageRoles)
        {
            if (!new RoleConsistent.Language().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in postRoles)
        {
            if (!new RoleConsistent.Post().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in glossaryRoles)
        {
            if (!new RoleConsistent.Glossary().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in notificationGroupRoles)
        {
            if (!new RoleConsistent.NotificationGroup().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in onboardingPageRoles)
        {
            if (!new RoleConsistent.OnboardingPage().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in userGroupRoles)
        {
            if (!new RoleConsistent.UserGroup().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        foreach (var role in userNotificationRoles)
        {
            if (!new RoleConsistent.UserNotification().Roles.Contains(role.Name))
            {
                _identityContext.Roles.Remove(role);
            }
        }
        // remove all roles that remain witch not associated to class in RoleConsistent
        foreach (var role in rolesAfterInitialize)
        {
            _identityContext.Roles.Remove(role);
        }
        await _identityContext.SaveChangesAsync();

        // Default Country
        var countries = _context.Countries.ToListAsync().Result;
        string defaultCountryId;
        if (!countries.Exists(n => n.Name == "Turkey"))
        {
            var defaultCountry = new Country { Name = "Turkey" };
            await _context.Countries.AddAsync(defaultCountry);
            await _context.SaveChangesAsync();
            defaultCountryId = defaultCountry.Id.ToString();
        }
        else
        {
            var defaultCountry = await _context.Countries.SingleOrDefaultAsync(c => c.Name == "Turkey");
            defaultCountryId = defaultCountry.Id.ToString();
        }

        // Default Languages
        var languages = _context.Languages.ToListAsync().Result;
        string defaultLanguageId;
        if (!languages.Exists(l => l.Name == "Turkish"))
        {
            var defaultLanguage = new Language { Name = "Turkish", Code ="tr" };
            await _context.Languages.AddAsync(defaultLanguage);
            await _context.SaveChangesAsync();
            defaultLanguageId = defaultLanguage.Id.ToString();
        }
        else
        {
            var defaultLanguage = await _context.Languages.SingleOrDefaultAsync(c => c.Name == "Turkish");
            defaultLanguageId = defaultLanguage.Id.ToString();
        }
        if (!languages.Exists(l => l.Name == "Arabic"))
        {
            var defaultLanguage = new Language { Name = "Arabic", Code = "ar" };
            await _context.Languages.AddAsync(defaultLanguage);
            await _context.SaveChangesAsync();
        }
        if (!languages.Exists(l => l.Name == "English"))
        {
            var defaultLanguage = new Language { Name = "English", Code = "en" };
            await _context.Languages.AddAsync(defaultLanguage);
            await _context.SaveChangesAsync();
        }
        // Default Notification
        var notifications = _context.Notifications.ToListAsync().Result;
        #region Post
        if (!notifications.Exists(n => n.Name == NotificationConsistent.Post.Add))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Post.Add });

        if (!notifications.Exists(n => n.Name == NotificationConsistent.Post.Edit))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Post.Edit });

        if (!notifications.Exists(n => n.Name == NotificationConsistent.Post.Delete))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Post.Delete });
        #endregion

        #region Grocery
        if (!notifications.Exists(n => n.Name == NotificationConsistent.Grocery.Add))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Grocery.Add });

        if (!notifications.Exists(n => n.Name == NotificationConsistent.Grocery.Edit))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Grocery.Edit });

        if (!notifications.Exists(n => n.Name == NotificationConsistent.Grocery.Delete))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Grocery.Delete });

        if (!notifications.Exists(n => n.Name == NotificationConsistent.Grocery.AddToFavorite))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Grocery.AddToFavorite });

        if (!notifications.Exists(n => n.Name == NotificationConsistent.Grocery.RemoveFromFavorite))
            _context.Notifications.Add(new Notification { Name = NotificationConsistent.Grocery.RemoveFromFavorite });
        #endregion
        await _context.SaveChangesAsync();

        var notificationGroups = _context.NotificationGroups.ToListAsync().Result;

        //Default NotificationGroup, // TODO: Add all default notifications and join with default user
        if (!notificationGroups.Exists(n => n.Name == "Master Notificationer"))
        {
            var masterNotificationGroup = new NotificationGroup { Name = "Master Notificationer" };
            _context.NotificationGroups.Add(masterNotificationGroup);
            var notificationsOfSystem = _context.Notifications.ToListAsync().Result;
            foreach (var item in notificationsOfSystem)
            {
                masterNotificationGroup.Notifications.Add(item);
            }
            await _context.SaveChangesAsync();
            
        }

        // Default Normal User NotificationGroup
        if (!notificationGroups.Exists(n => n.Name == "Normal Users Notificationer"))
        {
            var normalUsersNotificationGroup = new NotificationGroup { Name = "Normal Users Notificationer" };
            _context.NotificationGroups.Add(normalUsersNotificationGroup);
            var notificationsOfSystem = _context.Notifications.ToListAsync().Result;
            foreach (var item in notificationsOfSystem)
            {
                if (item.Name.StartsWith(@"Grocery\") || item.Name.StartsWith(@"Post\"))
                {
                    normalUsersNotificationGroup.Notifications.Add(item);
                }
            }
            await _context.SaveChangesAsync();
        }


        //Default Permission Group has all roles
        var applicationGroups =  await _identityContext.ApplicationGroups.ToListAsync();
        string masterGroupId;
        if (!applicationGroups.Exists(g => g.Name == "Master Group"))
        {
            var masterPermissionGroupRolesIds = _roleManager.Roles.ToListAsync().Result.Select(r => r.Id).ToList();
            var masterGroup = new ApplicationGroup
            {
                Name = "Master Group",
                Description = "Group have all roles",
            };
            await _identityContext.ApplicationGroups.AddAsync(masterGroup);
            await _identityContext.SaveChangesAsync();
            await _applicationGroupManager.SetGroupRolesByRolesIdsAsync(masterGroup.Id, masterPermissionGroupRolesIds.ToArray());
            masterGroupId = masterGroup.Id;
        }
        else
        {
            var masterGroup = await _identityContext.ApplicationGroups.SingleOrDefaultAsync(g => g.Name == "Master Group");
            // refresh Master Group roles
            var masterPermissionGroupRolesIds = _roleManager.Roles.ToListAsync().Result.Select(r => r.Id).ToList();
            await _applicationGroupManager.SetGroupRolesByRolesIdsAsync(masterGroup.Id, masterPermissionGroupRolesIds.ToArray());

            masterGroupId = masterGroup.Id;
        }
        //Default Permission Group for Normal User
        if (!applicationGroups.Exists(g => g.Name == "Normal User Group"))
        {
            var allRols = _roleManager.Roles.ToListAsync().Result.ToList();
            List<string> normalUserGroupRolesIds = new List<string>();
            // Grocery roles
            var groceryUserRoles = allRols.Where(r => r.Name == RoleConsistent.Grocery.AddToFavorite || 
                    r.Name == RoleConsistent.Grocery.RemoveFromFavorite || 
                    r.Name ==  RoleConsistent.Grocery.BrowseFavorite).ToList();
            groceryUserRoles.ForEach(r => normalUserGroupRolesIds.Add(r.Id));
            // Identity roles
            var identityUserRoles = allRols.Where(r => r.Name == RoleConsistent.Identity.ResetMyPassword).ToList();
            identityUserRoles.ForEach(r => normalUserGroupRolesIds.Add(r.Id));
            // Post roles
            var postUserRoles = allRols.Where(r => r.Name == RoleConsistent.Post.BrowseFavoritePosts).ToList();
            postUserRoles.ForEach(r => normalUserGroupRolesIds.Add(r.Id));
            // UserNotification roles
            var userNotificationUserRoles = allRols.Where(r => r.Name == RoleConsistent.UserNotification.Browse ||
                    r.Name == RoleConsistent.UserNotification.MakeAsRead ||
                    r.Name == RoleConsistent.UserNotification.MakeAsUnRead).ToList();
            userNotificationUserRoles.ForEach(r => normalUserGroupRolesIds.Add(r.Id));
            var normalUserGroup = new ApplicationGroup
            {
                Name = "Normal User Group",
                Description = "Group for normal users",
            };
            await _identityContext.ApplicationGroups.AddAsync(normalUserGroup);
            await _identityContext.SaveChangesAsync();
            await _applicationGroupManager.SetGroupRolesByRolesIdsAsync(normalUserGroup.Id, normalUserGroupRolesIds.ToArray());
        }
        else
        {
            //TODO: logic if we add new roles to normal user and (Normal User Group) already exited
        }


        // Default users
        var administrator = new ApplicationUser
        {
            UserName = "administrator@localhost",
            Email = "administrator@localhost",
            JobRole = JobRole.SuperAdmin,
            FullName = "Super Administrator",
            PhotoURL = "",//TODO : add default avatar photo
            CountryId = defaultCountryId,
            LanguageId = defaultLanguageId
        };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _applicationGroupManager.SetUserGroupsAsync(administrator.Id, masterGroupId);
            var masterNotificationGroup = await _context.NotificationGroups.SingleOrDefaultAsync(r => r.Name == "Master Notificationer");
            administrator.NotificationGroupIds.Add(masterNotificationGroup.Id.ToString());
            await _userManager.UpdateAsync(administrator);
            var userNotificationGroup = new UserNotificationGroup
            {
                UserId = administrator.Id,
                NotificationGroup = masterNotificationGroup,
            };
            _context.UserNotificationGroups.Add(userNotificationGroup);
            _context.SaveChanges();

        };

        #region Initialize Glossary

        InitializeGlossary();

        #endregion
    }

    private void InitializeGlossary()
    {
        // we read GlossaryConsistent class that contains all static elements of the application and add them to Db, may we add localization of the elements too 
        var turkishLanguageId = _context.Languages.FirstOrDefault(l => l.Code == "tr")!.Id;
        var arabicLanguageId = _context.Languages.FirstOrDefault(l => l.Code == "ar")!.Id;
        var englishLanguageId = _context.Languages.FirstOrDefault(l => l.Code == "en")!.Id;
        var glossaries = _context.Glossaries.ToListAsync().Result;
        #region Main
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Home.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Home.Key;
            glossary.Value = GlossaryConsistent.Main.Home.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "tr - Home" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الرئيسية" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Home" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.AboutUs.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.AboutUs.Key;
            glossary.Value = GlossaryConsistent.Main.AboutUs.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "tr - About us" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "من نحن" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "About us" });
            _context.SaveChanges();
        };

        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Favorite.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Favorite.Key;
            glossary.Value = GlossaryConsistent.Main.Favorite.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Favori" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "المفضلة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Favorite" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Profile.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Profile.Key;
            glossary.Value = GlossaryConsistent.Main.Profile.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Profil" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الملف الشخصي" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Profile" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.SearchStore.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.SearchStore.Key;
            glossary.Value = GlossaryConsistent.Main.SearchStore.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Mağazada ara" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "ابحث في المتجر" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Search Store" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Search.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Search.Key;
            glossary.Value = GlossaryConsistent.Main.Search.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Aramak" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "ابحث" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Search" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.SeeAll.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.SeeAll.Key;
            glossary.Value = GlossaryConsistent.Main.SeeAll.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Hepsini gör" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "شاهد الكل" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "See All" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Notification.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Notification.Key;
            glossary.Value = GlossaryConsistent.Main.Notification.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Bildirimler" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الإشعارات" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Notification" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.New.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.New.Key;
            glossary.Value = GlossaryConsistent.Main.New.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Yeni" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "جديد" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "New" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Today.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Today.Key;
            glossary.Value = GlossaryConsistent.Main.Today.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Bugün" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "اليوم" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Today" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Yesterday.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Yesterday.Key;
            glossary.Value = GlossaryConsistent.Main.Yesterday.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Dün" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الأمس" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Yesterday" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.MarkAllAsRead.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.MarkAllAsRead.Key;
            glossary.Value = GlossaryConsistent.Main.MarkAllAsRead.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Tümünü okundu olarak işaretle" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "وضع علامة تم القراءة للجميع" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Mark All As Read" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Groceries.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Groceries.Key;
            glossary.Value = GlossaryConsistent.Main.Groceries.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Bakkaliye" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "محلات البقالة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Groceries" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.AllGroceries.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.AllGroceries.Key;
            glossary.Value = GlossaryConsistent.Main.AllGroceries.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Tüm Market Ürünleri" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "جميع محلات البقالة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "All groceries" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.PopularGroceries.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.PopularGroceries.Key;
            glossary.Value = GlossaryConsistent.Main.PopularGroceries.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Popüler Marketler" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "محلات البقالة الشائعة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Popular Groceries" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.GroceriesStore.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.GroceriesStore.Key;
            glossary.Value = GlossaryConsistent.Main.GroceriesStore.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Manav" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "مخزن البقالة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Groceries store" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.All.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.All.Key;
            glossary.Value = GlossaryConsistent.Main.All.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Tüm" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الكل" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "All" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Active.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Active.Key;
            glossary.Value = GlossaryConsistent.Main.Active.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Aktif" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "النشطة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Active" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.InActive.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.InActive.Key;
            glossary.Value = GlossaryConsistent.Main.InActive.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Aktif Değil" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "غير النشطة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "InActive" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.RecentlyAdded.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.RecentlyAdded.Key;
            glossary.Value = GlossaryConsistent.Main.RecentlyAdded.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Son Eklenenler" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "المضافة حديثاً" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Recently Added" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.SelectCountry.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.SelectCountry.Key;
            glossary.Value = GlossaryConsistent.Main.SelectCountry.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Ülke Seçiniz" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "اختر البلد" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Select Country" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.SelectLanguage.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.SelectLanguage.Key;
            glossary.Value = GlossaryConsistent.Main.SelectLanguage.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Dil Seçin" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "اختر اللغة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Select Language" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.Back.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.Back.Key;
            glossary.Value = GlossaryConsistent.Main.Back.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "geriye doğru" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "للخلف" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Back" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Main.GetStarted.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Main.GetStarted.Key;
            glossary.Value = GlossaryConsistent.Main.GetStarted.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Başlayın" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "هيا نبدأ" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Get Started" });
            _context.SaveChanges();
        };
        #endregion

        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.Introduction.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.Introduction.Key;
            glossary.Value = GlossaryConsistent.Identity.Introduction.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Favorilere eklemek ve yeni gönderi bildirimleri almak için bir hesabınızın olması gerekir" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "كي تستطيع الإضافة للمفضلة وتلقي الإشعارات حول الإعلانات يجب أن يكون لديك حساب" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "To make favorite and receive new posts notifications you need to have an account" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.SignIn.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.SignIn.Key;
            glossary.Value = GlossaryConsistent.Identity.SignIn.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Kayıt olmak" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "تسجيل الدخول" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Sign In" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.SignUp.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.SignUp.Key;
            glossary.Value = GlossaryConsistent.Identity.SignUp.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Üye olmak" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "التسجيل" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Sign Up" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.ContinueWithoutAccount.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.ContinueWithoutAccount.Key;
            glossary.Value = GlossaryConsistent.Identity.ContinueWithoutAccount.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Hesapsız Devam Et" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الاستمرار بدون حساب" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Continue Without Account" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.WelcomeMissed.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.WelcomeMissed.Key;
            glossary.Value = GlossaryConsistent.Identity.WelcomeMissed.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "MERHABA! Tekrar hoş geldiniz, özlendiniz" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "أهلا بعودتك مجدداً، لقد افتقدناك" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Hi! Welcome back, you’ve been missed" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.Email.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.Email.Key;
            glossary.Value = GlossaryConsistent.Identity.Email.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "E-posta" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "البريد الإلكتروني" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Email" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.EmailExample.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.EmailExample.Key;
            glossary.Value = GlossaryConsistent.Identity.EmailExample.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "example@gmail.com" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "example@gmail.com" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "example@gmail.com" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.Password.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.Password.Key;
            glossary.Value = GlossaryConsistent.Identity.Password.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Şifre" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "كلمة المرور" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Password" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.ForgotPassword.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.ForgotPassword.Key;
            glossary.Value = GlossaryConsistent.Identity.ForgotPassword.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Şifremi Unuttum" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "نسيت كلمة المرور" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Forgot Password" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.OrSignInWith.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.OrSignInWith.Key;
            glossary.Value = GlossaryConsistent.Identity.OrSignInWith.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Veya şununla oturum açın" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "أو سجل باستخدام" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Or sign in with" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.DoNotHaveAnAccount.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.DoNotHaveAnAccount.Key;
            glossary.Value = GlossaryConsistent.Identity.DoNotHaveAnAccount.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Hesabınız yok mu?" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "لا تملك حساباً ؟" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Don’t have an account?" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.BasicInfo.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.BasicInfo.Key;
            glossary.Value = GlossaryConsistent.Identity.BasicInfo.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Temel Bilgiler" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "المعلومات الأساسية" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Basic Info" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.DarkMode.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.DarkMode.Key;
            glossary.Value = GlossaryConsistent.Identity.DarkMode.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Karanlık Mod" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الوضع الليلي" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Dark Mode" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.FullName.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.FullName.Key;
            glossary.Value = GlossaryConsistent.Identity.FullName.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Ad Soyad" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الاسم الكامل" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Full Name" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.DateOfBirth.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.DateOfBirth.Key;
            glossary.Value = GlossaryConsistent.Identity.DateOfBirth.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Doğum tarihi" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "تاريخ الميلاد" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Date of Birth" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.EmailAddress.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.EmailAddress.Key;
            glossary.Value = GlossaryConsistent.Identity.EmailAddress.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "E-posta Adresi" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "عنوان البريد الإلكتروني" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Email Address" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.PhoneNumber.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.PhoneNumber.Key;
            glossary.Value = GlossaryConsistent.Identity.PhoneNumber.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Telefon numarası" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "رقم الهاتف" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Phone Number" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.Language.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.Language.Key;
            glossary.Value = GlossaryConsistent.Identity.Language.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Dil" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "اللغة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Language" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.Country.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.Country.Key;
            glossary.Value = GlossaryConsistent.Identity.Country.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Ülke" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "البلد" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Country" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.ResetPassword.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.ResetPassword.Key;
            glossary.Value = GlossaryConsistent.Identity.ResetPassword.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Şifreyi yenile" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "إعادة تعيين كلمة المرور" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Reset Password" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.ConfirmPassword.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.ConfirmPassword.Key;
            glossary.Value = GlossaryConsistent.Identity.ConfirmPassword.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Şifreyi onayla" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "تأكيد كلمة المرور" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Confirm Password" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.NewPassword.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.NewPassword.Key;
            glossary.Value = GlossaryConsistent.Identity.NewPassword.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Yeni Şifre" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "كلمة مرور جديدة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "New Password" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.CreateNewPassword.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.CreateNewPassword.Key;
            glossary.Value = GlossaryConsistent.Identity.CreateNewPassword.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Yeni şifre oluştur" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "إنشاء كلمة مرور جديدة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Create new password" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.CreateNewPasswordMessage.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.CreateNewPasswordMessage.Key;
            glossary.Value = GlossaryConsistent.Identity.CreateNewPasswordMessage.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Yeni şifreniz daha önce kullandığınız şifreden farklı olmalıdır." });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "كلمة المرور الجديدة يجب أن تكون مختلفة عن الكلمة السابقة" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Your new password must be different from previously used password." });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.SaveChanges.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.SaveChanges.Key;
            glossary.Value = GlossaryConsistent.Identity.SaveChanges.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Değişiklikleri Kaydet" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "حفظ التغييرات" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Save Changes" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.CreateAccount.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.CreateAccount.Key;
            glossary.Value = GlossaryConsistent.Identity.CreateAccount.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Hesap oluşturmak" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "إنشاء حساب" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Create Account" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.CreateAccountMessage.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.CreateAccountMessage.Key;
            glossary.Value = GlossaryConsistent.Identity.CreateAccountMessage.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Aşağıya bilgilerinizi girin veya sosyal hesabınızla kayıt olun." });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "إملأ معلوماتك هنا أو سجل باستخدام حسابك على مواقع التواصل الاجتماعي" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Fill your information below or register with your social account." });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.AgreeWith.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.AgreeWith.Key;
            glossary.Value = GlossaryConsistent.Identity.AgreeWith.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Katılıyorum" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "موافق على" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Agree With" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.TermsCondition.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.TermsCondition.Key;
            glossary.Value = GlossaryConsistent.Identity.TermsCondition.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Şartlar ve koşullar" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الشروط والإتفاقيات" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Terms & Condition" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.VerifyCode.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.VerifyCode.Key;
            glossary.Value = GlossaryConsistent.Identity.VerifyCode.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Doğrulama kodu" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "تأكيد الرمز" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Verify Code" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.VerifyCodeMessage.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.VerifyCodeMessage.Key;
            glossary.Value = GlossaryConsistent.Identity.VerifyCodeMessage.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "lütfen az önce e-postaya gönderdiğimiz kodu girin" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "الرجاء ادخال رمز التحقق الذي تم ارسالة الى الإيميل" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "please enter the code we just sent to email" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.Verify.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.Verify.Key;
            glossary.Value = GlossaryConsistent.Identity.Verify.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Doğrulamak" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "تأكيد" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Verify" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.DidNotReceiveOTP.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.DidNotReceiveOTP.Key;
            glossary.Value = GlossaryConsistent.Identity.DidNotReceiveOTP.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Doğrulama kodu almadınız" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "لم تستلم رمز التحقق ؟" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Didn’t receive OTP?" });
            _context.SaveChanges();
        };
        if (!glossaries.Exists(g => g.Key == GlossaryConsistent.Identity.ResendCode.Key))
        {
            var glossary = new Glossary();
            glossary.Key = GlossaryConsistent.Identity.ResendCode.Key;
            glossary.Value = GlossaryConsistent.Identity.ResendCode.Value;
            _context.Glossaries.Add(glossary);
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = turkishLanguageId, Value = "Yeniden gönderme kodu" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = arabicLanguageId, Value = "أعد إرسال رمز التحقق" });
            _context.GlossariesLocalization.Add(new GlossaryLocalization { GlossaryId = glossary.Id, LanguageId = englishLanguageId, Value = "Resend Code" });
            _context.SaveChanges();
        };
    }
}
