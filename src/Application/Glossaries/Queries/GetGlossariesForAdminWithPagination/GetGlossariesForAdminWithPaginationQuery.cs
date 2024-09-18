using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossariesForAdminWithPagination;
public class GetGlossariesForAdminWithPaginationQuery: IRequest<PaginatedList<GetGlossariesForAdminWithPaginationQueryDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
}

public class GetGlossariesForAdminWithPaginationQueryHandler : IRequestHandler<GetGlossariesForAdminWithPaginationQuery, PaginatedList<GetGlossariesForAdminWithPaginationQueryDto>>
{
    private readonly ILogger<GetGlossariesForAdminWithPaginationQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IGlossaryRepository _glossaryRepository;
    private readonly IGlossaryLocalizationRepository _glossaryLocalizationRepository;
    private readonly ILanguageRepository _languageRepository;

    public GetGlossariesForAdminWithPaginationQueryHandler(ILogger<GetGlossariesForAdminWithPaginationQueryHandler> logger,
                                                           IMapper mapper,
                                                           IGlossaryRepository glossaryRepository,
                                                           IGlossaryLocalizationRepository glossaryLocalizationRepository,
                                                           ILanguageRepository languageRepository)
    {
        _logger = logger;
        _mapper = mapper;
        _glossaryRepository = glossaryRepository;
        _glossaryLocalizationRepository = glossaryLocalizationRepository;
        _languageRepository = languageRepository;
    }

    public async Task<PaginatedList<GetGlossariesForAdminWithPaginationQueryDto>> Handle(GetGlossariesForAdminWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // get all glossaries
        var glossaries = _glossaryRepository.GetAll();
        // apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            glossaries = glossaries.Where(x => x.Key.ToLower().Contains(request.SearchText.ToLower()));

        // map result and paginated it
        var result = await glossaries
            .OrderBy(g => g.Key)
            .ProjectTo<GetGlossariesForAdminWithPaginationQueryDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        //fill localization for all Glossary in result
        await LocalizationHelper.FillGlossaryLocalizations(result, _glossaryLocalizationRepository, _mapper);

        // fill key and language name for localization of all returned glossary
        foreach (var glossaryDto in result.Items)
        {
            foreach (var localizationDto in glossaryDto.GlossaryLocalizationDtos)
            {
                localizationDto.LanguageName = _languageRepository.GetByIdAsync(localizationDto.LanguageId).Result.Name;
                localizationDto.Key = _glossaryRepository.GetByIdAsync(localizationDto.GlossaryId).Result.Key;
            }
        }
        return result;
    }
}
