using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class NotificationGroupConfiguration : IEntityTypeConfiguration<NotificationGroup>
{
    public void Configure(EntityTypeBuilder<NotificationGroup> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .HasMany(l => l.Notifications)
            .WithMany(p => p.NotificationGroups)
            .UsingEntity<NotificationGroupDetail>();
    }
}
