using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class OnboardingPageLocalizationConfiguration : IEntityTypeConfiguration<OnboardingPageLocalization>
{
    public void Configure(EntityTypeBuilder<OnboardingPageLocalization> builder)
    {
        builder.HasKey(c => c.Id);

    }
}
