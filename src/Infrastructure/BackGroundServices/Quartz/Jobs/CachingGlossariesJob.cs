using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Languages;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Infrastructure.Repositories;
using Offers.CleanArchitecture.Infrastructure.Services;
using Quartz;

namespace Offers.CleanArchitecture.Infrastructure.BackGroundServices.Quartz.Jobs;
public class CachingGlossariesJob : IJob // this job responsible for caching Glossaries and them localization in memory
{
    private readonly ILogger<CachingGlossariesJob> _logger;
    private readonly ICacheService _cacheService;
    private readonly IGlossaryRepository _glossaryRepository;
    private readonly IGlossaryLocalizationRepository _glossaryLocalizationRepository;
    private readonly ILanguageRepository _languageRepository;
    private readonly IMapper _mapper;

    public CachingGlossariesJob(ILogger<CachingGlossariesJob> logger,
                               ICacheService cacheService,
                               IGlossaryRepository glossaryRepository,
                               IGlossaryLocalizationRepository glossaryLocalizationRepository,
                               ILanguageRepository languageRepository,
                               IMapper mapper)
    {
        _logger = logger;
        _cacheService = cacheService;
        _glossaryRepository = glossaryRepository;
        _glossaryLocalizationRepository = glossaryLocalizationRepository;
        _languageRepository = languageRepository;
        _mapper = mapper;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        // first remove previous cached data if existed from memory 
        string languagesKey = "languages";
        var languagesInMemory = _cacheService.GetData<List<Language>>(languagesKey);
        if (languagesInMemory != null)
        {
            _cacheService.RemoveData(languagesKey);
        }

        // //get languages and save them in memory
        var languages = await _languageRepository.GetAll().ToListAsync();
        
        _cacheService.SetData<List<Language>>(languagesKey, languages, DateTimeOffset.Now.AddDays(7));

        // first remove previous cached data if existed from memory 
        string glossariesKey = "glossaries";
        var glossariesInMemory = _cacheService.GetData<List<Glossary>>(glossariesKey);
        if (glossariesInMemory != null)
        {
            _cacheService.RemoveData(glossariesKey);
        }
        // get glossaries and save them in memory
        var glossaries = await _glossaryRepository.GetAll().ToListAsync();

       
        _cacheService.SetData<List<Glossary>>(glossariesKey, glossaries, DateTimeOffset.Now.AddDays(7));

        //get GlossaryLocalization for each language and save them in memory
        foreach (var language in languages)
        {
            List<GlossaryLocalization> glossaryLocalizationsCaching = await _glossaryLocalizationRepository.GetAll()
                .Where(g => g.LanguageId == language.Id)
                .ToListAsync();
            string glossaryLocalizationKey = "glossaries-" + language.Id.ToString();
            var existingValue = _cacheService.GetData<List<GlossaryLocalization>>(glossaryLocalizationKey);
            if (existingValue != null)
            {
                _cacheService.RemoveData(glossaryLocalizationKey);
            }
            _cacheService.SetData<List<GlossaryLocalization>>(glossaryLocalizationKey, glossaryLocalizationsCaching, DateTimeOffset.Now.AddDays(7));

        }

    }
}
