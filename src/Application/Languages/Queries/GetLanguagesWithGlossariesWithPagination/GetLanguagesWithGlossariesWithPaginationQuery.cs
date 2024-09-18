using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithGlossariesWithPagination;
public class GetLanguagesWithGlossariesWithPaginationQuery : IRequest<string>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }

}

public class GetLanguagesWithGlossariesWithPaginationQueryHandler : IRequestHandler<GetLanguagesWithGlossariesWithPaginationQuery, string>
{
    private readonly ILogger<GetLanguagesWithGlossariesWithPaginationQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly ILanguageRepository _languageRepository;
    private readonly IGlossaryLocalizationRepository _glossaryLocalizationRepository;
    private readonly IGlossaryRepository _glossaryRepository;

    public GetLanguagesWithGlossariesWithPaginationQueryHandler(ILogger<GetLanguagesWithGlossariesWithPaginationQueryHandler> logger,
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

    public async Task<string> Handle(GetLanguagesWithGlossariesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // get all languages
        var languages = _languageRepository.GetAll();
        if (request.SearchText != null)
            languages = languages.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower()) || x.Code.ToLower().Contains(request.SearchText.ToLower()));
        // map languages to Dto
        //var result = await languages.OrderBy(l=>l.Name).ProjectToListAsync<GetLanguagesWithGlossariesWithPaginationQueryDto,Language>(_mapper.ConfigurationProvider);
        var result = await languages
            .OrderBy(l => l.Name)
            .ProjectTo<GetLanguagesWithGlossariesWithPaginationQueryDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        // get all glossary of each language and fill the Dto Glossary list
        foreach (var languageWithGlossariesDto in result.Items)
        {
            var languageGlossaries = await _glossaryLocalizationRepository.GetAllByLanguageIdAsync(languageWithGlossariesDto.Id);
            foreach (var glossaryLocalization in languageGlossaries) 
            {
                var key = _glossaryRepository.GetByIdAsync(glossaryLocalization.GlossaryId).Result.Key;
                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(key, glossaryLocalization.Value);
                languageWithGlossariesDto.Glossary.Add(pair);
            }

        }

        var response = JsonBuilder.BuildLanguagesGlossariesJson(result);

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true
        };


        return JsonSerializer.Serialize<dynamic>(JsonSerializer.Deserialize<dynamic>(response, options), options);
    }
}
