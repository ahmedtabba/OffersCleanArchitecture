using System.Runtime.InteropServices.Marshalling;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.Db;

public interface IApplicationDbContext
{
    DbSet<Grocery> Groceries { get; }
    DbSet<Post> Posts { get; }
    DbSet<FavoraiteGrocery> FavoraiteGroceries { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<Language> Languages { get; }
    public DbSet<PostLocalization> PostLocalization { get; }
    public DbSet<GroceryLocalization> GroceryLocalization { get; }
    public DbSet<Notification> Notifications { get; }
    public DbSet<NotificationGroup> NotificationGroups { get; }
    public DbSet<NotificationGroupDetail> NotificationGroupDetails { get; }
    public DbSet<UserNotification> UserNotifications { get; }
    public DbSet<UserNotificationGroup> UserNotificationGroups { get; }
    public DbSet<OnboardingPage> OnboardingPages { get; }
    public DbSet<OnboardingPageLocalization> OnboardingPageLocalization { get; }
    public DbSet<Glossary> Glossaries { get; }
    public DbSet<GlossaryLocalization> GlossariesLocalization { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

//public interface IIdentityDbContext
//{

//    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
//}
