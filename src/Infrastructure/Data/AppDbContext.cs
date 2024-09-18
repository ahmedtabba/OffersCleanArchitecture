using System.Reflection;
using Offers.CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.Db;

namespace Offers.CleanArchitecture.Infrastructure.Data;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Grocery> Groceries => Set<Grocery>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<FavoraiteGrocery> FavoraiteGroceries => Set<FavoraiteGrocery>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Language> Languages => Set<Language>();

    public DbSet<PostLocalization> PostLocalization => Set<PostLocalization>();
    public DbSet<GroceryLocalization> GroceryLocalization => Set<GroceryLocalization>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<NotificationGroup> NotificationGroups => Set<NotificationGroup>();

    public DbSet<NotificationGroupDetail> NotificationGroupDetails => Set<NotificationGroupDetail>();

    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    public DbSet<UserNotificationGroup> UserNotificationGroups => Set<UserNotificationGroup>();

    public DbSet<OnboardingPage> OnboardingPages => Set<OnboardingPage>();

    public DbSet<OnboardingPageLocalization> OnboardingPageLocalization => Set<OnboardingPageLocalization>();

    public DbSet<Glossary> Glossaries => Set<Glossary>();

    public DbSet<GlossaryLocalization> GlossariesLocalization => Set<GlossaryLocalization>();


    //public DbSet<LanguagePost> LanguagePosts => Set<LanguagePost>();
    //public DbSet<LanguageGrocery> LanguageGroceries => Set<LanguageGrocery>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<Language>()
            .HasMany(l => l.Groceries)
            .WithMany(g => g.Languages)
            .UsingEntity<GroceryLocalization>();

        builder.Entity<Language>()
            .HasMany(l => l.Posts)
            .WithMany(p => p.Languages)
            .UsingEntity<PostLocalization>();

        builder.Entity<Language>()
            .HasMany(l => l.OnboardingPages)
            .WithMany(op => op.Languages)
            .UsingEntity<OnboardingPageLocalization>();

        builder.Entity<Language>()
            .HasMany(l => l.Glossaries)
            .WithMany(g => g.Languages)
            .UsingEntity<GlossaryLocalization>();

        //builder.Entity<NotificationGroup>()
        //    .HasMany(l => l.Notifications)
        //    .WithMany(p => p.NotificationGroups)
        //    .UsingEntity<NotificationGroupDetail>();

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        base.OnConfiguring(optionsBuilder);
    }

}


