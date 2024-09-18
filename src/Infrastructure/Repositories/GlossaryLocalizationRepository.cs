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
public class GlossaryLocalizationRepository : BaseRepository<GlossaryLocalization>, IGlossaryLocalizationRepository
{
    public GlossaryLocalizationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<GlossaryLocalization>> GetAllByLanguageIdAsync(Guid languageId)
    {
        return await DbContext.GlossariesLocalization
            .Where(gl => gl.LanguageId == languageId)
            .ToListAsync();
    }

    public async Task<List<GlossaryLocalization>> GetGlossaryLocalizationByGlossaryIdAsync(Guid glossaryId)
    {
        return await DbContext.GlossariesLocalization
            .Where(gl=>gl.GlossaryId == glossaryId)
            .ToListAsync();
    }
}
