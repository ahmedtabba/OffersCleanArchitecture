using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class OnboardingPageConfiguration : IEntityTypeConfiguration<OnboardingPage>
{
    public void Configure(EntityTypeBuilder<OnboardingPage> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).HasMaxLength(20);
        builder.Property(c => c.Description).HasMaxLength(55);

    }
}
