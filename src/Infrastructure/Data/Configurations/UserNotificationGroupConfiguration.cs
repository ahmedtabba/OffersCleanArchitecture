using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class UserNotificationGroupConfiguration : IEntityTypeConfiguration<UserNotificationGroup>
{
    public void Configure(EntityTypeBuilder<UserNotificationGroup> builder)
    {
        builder.HasKey(c => c.Id);
    }
}
