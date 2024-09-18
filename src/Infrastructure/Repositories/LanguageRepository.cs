using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Data;

namespace Offers.CleanArchitecture.Infrastructure.Repositories;
public class LanguageRepository : BaseRepository<Language>, ILanguageRepository
{
    public LanguageRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Language> GetLanguageByCodeAsync(string code)
    {
        return await DbContext.Languages.FirstOrDefaultAsync(l => l.Code == code);
    }
}
