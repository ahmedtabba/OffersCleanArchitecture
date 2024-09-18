using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Repositories.CachedRepositories;
public class CachedGlossaryLocalizationRepository : IGlossaryLocalizationRepository
{
    private readonly ICacheService _cacheService;
    private readonly IGlossaryLocalizationRepository _decorated;

    public CachedGlossaryLocalizationRepository(ICacheService cacheService,
                                                IGlossaryLocalizationRepository glossaryLocalizationRepository)
    {
        _cacheService = cacheService;
        _decorated = glossaryLocalizationRepository;
    }

    public Task<GlossaryLocalization> AddAsync(GlossaryLocalization entity) => _decorated.AddAsync(entity);


    public Task DeleteAsync(GlossaryLocalization entity) => _decorated.DeleteAsync(entity);
  

    public IQueryable<GlossaryLocalization> GetAll() => _decorated.GetAll();


    public IQueryable<GlossaryLocalization> GetAllAsTracking() => _decorated.GetAllAsTracking();

    public async Task<List<GlossaryLocalization>> GetAllByLanguageIdAsync(Guid languageId)
    {
        string key = "glossaries-" + languageId.ToString();
        var GlossariesCached = _cacheService.GetData<List<GlossaryLocalization>>(key);
        if (GlossariesCached == null)
        {
            return await _decorated.GetAllByLanguageIdAsync(languageId);
        }
        else
        {
            return GlossariesCached;
        }
    }

    public Task<GlossaryLocalization> GetByIdAsync(Guid id) => _decorated.GetByIdAsync(id);


    public Task<List<GlossaryLocalization>> GetGlossaryLocalizationByGlossaryIdAsync(Guid glossaryId) => _decorated.GetGlossaryLocalizationByGlossaryIdAsync(glossaryId);
  

    public Task UpdateAsync(GlossaryLocalization entity) => _decorated.UpdateAsync(entity);
   
}
