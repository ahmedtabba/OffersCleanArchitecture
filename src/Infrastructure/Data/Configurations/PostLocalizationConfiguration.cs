using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class PostLocalizationConfiguration : IEntityTypeConfiguration<PostLocalization>
{
    public void Configure(EntityTypeBuilder<PostLocalization> builder)
    {
        builder.HasKey(c => c.Id);

    }
}
