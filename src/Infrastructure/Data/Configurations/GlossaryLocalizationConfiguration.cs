using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class GlossaryLocalizationConfiguration : IEntityTypeConfiguration<GlossaryLocalization>
{
    public void Configure(EntityTypeBuilder<GlossaryLocalization> builder)
    {
        builder.HasKey(c => c.Id);

    }
}
