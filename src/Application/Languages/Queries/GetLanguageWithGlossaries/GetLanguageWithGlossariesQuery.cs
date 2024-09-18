using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguageWithGlossaries;
public class GetLanguageWithGlossariesQuery : IRequest<string>
{
    public string LanguageCode { get; set; } = null!;
}

public class GetLanguageWithGlossariesQueryHandler : IRequestHandler<GetLanguageWithGlossariesQuery, string>
{
    private readonly ILogger<GetLanguageWithGlossariesQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly ILanguageRepository _languageRepository;
    private readonly IGlossaryLocalizationRepository _glossaryLocalizationRepository;
    private readonly IGlossaryRepository _glossaryRepository;

    public GetLanguageWithGlossariesQueryHandler(ILogger<GetLanguageWithGlossariesQueryHandler> logger,
                                                  IMapper mapper,
                                                  ILanguageRepository languageRepository,
                                                  IGlossaryLocalizationRepository glossaryLocalizationRepository,
                                                  IGlossaryRepository glossaryRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _languageRepository = languageRepository;
        _glossaryLocalizationRepository = glossaryLocalizationRepository;
        _glossaryRepository = glossaryRepository;
    }

    public async Task<string> Handle(GetLanguageWithGlossariesQuery request, CancellationToken cancellationToken)
    {
        var language = await _languageRepository.GetLanguageByCodeAsync(request.LanguageCode);
        // map language to Dto
        var languageWithGlossaries = _mapper.Map<GetLanguageWithGlossariesQueryDto>(language);
        // get glossaries of language
        var languageGlossaries = await _glossaryLocalizationRepository.GetAllByLanguageIdAsync(languageWithGlossaries.Id);

        foreach(var glossaryLocalization in languageGlossaries)
        {
            var key = _glossaryRepository.GetByIdAsync(glossaryLocalization.GlossaryId).Result.Key;
            KeyValuePair<string, string> pair = new KeyValuePair<string, string>(key, glossaryLocalization.Value);
            languageWithGlossaries.Glossary.Add(pair);
        }

        var response = JsonBuilder.BuildLanguagesGlossariesJson(languageWithGlossaries);
        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true
        };


        return JsonSerializer.Serialize<dynamic>(JsonSerializer.Deserialize<dynamic>(response, options), options);
    }
}
