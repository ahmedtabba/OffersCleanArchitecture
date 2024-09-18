using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class GlossaryRepository : BaseRepository<Glossary>, IGlossaryRepository
{
    public GlossaryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Glossary> GetGlossaryByKeyAsync(string key)
    {
        return await DbContext.Glossaries.FirstOrDefaultAsync(g => g.Key == key);
    }
}
