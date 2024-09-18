using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
public interface ILanguageRepository : IRepositoryAsync<Language>
{
    public Task<Language> GetLanguageByCodeAsync(string code);
}
