using System.Reflection;
using Offers.CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Offers.CleanArchitecture.Infrastructure.Data;

public class CleanArchitectureIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public CleanArchitectureIdentityDbContext(DbContextOptions<CleanArchitectureIdentityDbContext> options) : base(options)
    {

    }
    public virtual DbSet<ApplicationGroup> ApplicationGroups { get; set; }
    //public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        /* // Configure the ApplicationUser to ApplicationUserGroupHelper relationship
         builder.Entity<ApplicationUser>()
             .HasMany(u => u.Groups) // Assuming ApplicationUser has a property 'Groups' of type ICollection<IApplicationUserGroup>
             .WithOne(ug => ug.ApplicationUser) // Assuming ApplicationUserGroupHelper has a property 'ApplicationUser' of type ApplicationUser
             .HasForeignKey(ug => ug.ApplicationUserId) // Assuming ApplicationUserGroupHelper has a property 'ApplicationUserId'
             .IsRequired();
        */

        // Many to Many => ApplicationGroup <=> ApplicationUser
        builder.Entity<ApplicationGroup>()
            .HasMany(g => g.ApplicationUsers)
            .WithMany(u => u.Groups)
            .UsingEntity<ApplicationUserGroup>();

        builder.Entity<ApplicationUserGroup>()
                .ToTable("ApplicationUserGroups")
                .HasKey(ug => ug.Id);

        // Assuming ApplicationUserGroupHelper and ApplicationGroupRole entities are correctly set up
        // You might need to configure similar relationships for them

        // Many to Many => ApplicationGroup <=> ApplicationRole
        builder.Entity<ApplicationGroup>()
            .HasMany(g => g.ApplicationRoles)
            .WithMany(gr => gr.Groups) 
            .UsingEntity<ApplicationGroupRole>();

        builder.Entity<ApplicationGroupRole>()
               .ToTable("ApplicationGroupRoles")
               .HasKey(gr => gr.Id);


        base.OnModelCreating(builder);
    }


}


