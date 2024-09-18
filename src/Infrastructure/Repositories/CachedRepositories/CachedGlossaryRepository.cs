using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Infrastructure.Repositories.CachedRepositories;
public class CachedGlossaryRepository : IGlossaryRepository
{
    private readonly IGlossaryRepository _decorated;
    private readonly ICacheService _cacheService;

    public CachedGlossaryRepository(IGlossaryRepository glossaryRepository,
                                   ICacheService cacheService)
    {
        _decorated = glossaryRepository;
        _cacheService = cacheService;
    }

    public Task<Glossary> AddAsync(Glossary entity) => _decorated.AddAsync(entity);


    public Task DeleteAsync(Glossary entity) => _decorated.DeleteAsync(entity);


    public IQueryable<Glossary> GetAll()
    {
        var key = "glossaries";
        var glossariesCached = _cacheService.GetData<List<Glossary>>(key);
        if (glossariesCached == null)
        {
            return _decorated.GetAll();
        }
        return glossariesCached.AsQueryable();
    }


    public IQueryable<Glossary> GetAllAsTracking() => _decorated.GetAllAsTracking();


    public async Task<Glossary> GetByIdAsync(Guid id)
    {
        var key = "glossaries";
        var glossariesCached = _cacheService.GetData<List<Glossary>>(key);
        if (glossariesCached == null)
        {
            return await _decorated.GetByIdAsync(id);
        }
        var glossary = glossariesCached.FirstOrDefault(g => g.Id == id);
        return glossary;

    }

    public Task<Glossary> GetGlossaryByKeyAsync(string key) => _decorated.GetGlossaryByKeyAsync(key);


    public Task UpdateAsync(Glossary entity) => _decorated.UpdateAsync(entity);

}
