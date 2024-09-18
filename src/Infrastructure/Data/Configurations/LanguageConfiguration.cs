using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class LanguageConfiguration
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Code).HasMaxLength(3).IsRequired();

        //builder
        //    .HasMany(l => l.Groceries)
        //    .WithMany(g => g.Languages)
        //    .UsingEntity<GroceryLocalization>();

        //builder.HasMany(l => l.Groceries)
        //    .WithMany(g => g.Languages)
        //    .UsingEntity<GroceryLocalization>(
        //    l => l.HasOne<Grocery>(e => e.Grocery).WithMany(e => e.GroceriesLocalization),
        //    r => r.HasOne<Language>(e => e.Language).WithMany(e => e.GroceriesLocalization));


        //builder
        //    .HasMany(l => l.Posts)
        //    .WithMany(p => p.Languages)
        //    .UsingEntity<PostLocalization>();

        //builder.HasMany(l => l.Posts)
        //    .WithMany(p => p.Languages)
        //    .UsingEntity<LanguagePost>(
        //    l => l.HasOne<Post>(e => e.Post).WithMany(e => e.LanguagePosts),
        //    r => r.HasOne<Language>(e => e.Language).WithMany(e => e.LanguagePosts));
    }
}

