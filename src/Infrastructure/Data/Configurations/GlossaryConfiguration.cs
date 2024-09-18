﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Data.Configurations;
public class GlossaryConfiguration : IEntityTypeConfiguration<Glossary>
{
    public void Configure(EntityTypeBuilder<Glossary> builder)
    {
        builder.HasKey(c => c.Id);
      
    }
}
