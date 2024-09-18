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
public class CachedLanguageRepository : ILanguageRepository
{
    private readonly ICacheService _cacheService;
    private readonly ILanguageRepository _decorated;

    public CachedLanguageRepository(ICacheService cacheService,
                                    ILanguageRepository languageRepository)
    {
        _cacheService = cacheService;
        _decorated = languageRepository;
    }
    public Task<Language> AddAsync(Language entity) => _decorated.AddAsync(entity);


    public Task DeleteAsync(Language entity) => _decorated.DeleteAsync(entity);


    public IQueryable<Language> GetAll()
    {
        var key = "languages";
        var languagesCached = _cacheService.GetData<List<Language>>(key);
        if (languagesCached == null)
        {
            return _decorated.GetAll();
        }
        else
        {
            return languagesCached.AsQueryable();
        }
    }

    public IQueryable<Language> GetAllAsTracking() => _decorated.GetAllAsTracking();


    public async Task<Language> GetByIdAsync(Guid id)
    {
        var key = "languages";
        var languagesCached = _cacheService.GetData<List<Language>>(key);
        if (languagesCached == null)
        {
            return await _decorated.GetByIdAsync(id);
        }
        else
            return languagesCached.FirstOrDefault(l => l.Id == id);
    }


    public async Task<Language> GetLanguageByCodeAsync(string code)
    {
        var key = "languages";
        var languagesCached = _cacheService.GetData<List<Language>>(key);
        if (languagesCached == null)
        {
            return await _decorated.GetLanguageByCodeAsync(code);
        }
        else
            return languagesCached.FirstOrDefault(l => l.Code == code);
        
    }

    public Task UpdateAsync(Language entity) => _decorated.UpdateAsync(entity);

}
