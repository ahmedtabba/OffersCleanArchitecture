using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class GroceryConfiguration : IEntityTypeConfiguration<Grocery>
{
    public void Configure(EntityTypeBuilder<Grocery> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
