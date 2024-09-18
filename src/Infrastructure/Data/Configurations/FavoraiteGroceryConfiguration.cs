using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class FavoraiteGroceryConfiguration : IEntityTypeConfiguration<FavoraiteGrocery>
{
    public void Configure(EntityTypeBuilder<FavoraiteGrocery> builder)
    {
        builder.HasKey(fg => fg.Id);
        //builder.HasOne(fg => fg.Grocery)
        //    .WithMany(g => g.FavoraiteGroceries)
        //    .HasForeignKey(fg => fg.GroceryId);
    }
}
