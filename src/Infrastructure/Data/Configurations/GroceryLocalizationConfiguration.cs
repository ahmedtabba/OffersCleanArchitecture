using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class GroceryLocalizationConfiguration : IEntityTypeConfiguration<GroceryLocalization>
{
    public void Configure(EntityTypeBuilder<GroceryLocalization> builder)
    {
        builder.HasKey(c => c.Id);

    }
}
