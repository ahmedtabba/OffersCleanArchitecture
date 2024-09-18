using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Glossaries.Queries.GetGlossary;
public class GetGlossaryForAdminQuery : IRequest<GetGlossaryForAdminQueryDto>
{
    public Guid GlossaryId { get; set; }
}

public class GetGlossaryForAdminQueryHandler : IRequestHandler<GetGlossaryForAdminQuery, GetGlossaryForAdminQueryDto>
{
    private readonly ILogger<GetGlossaryForAdminQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IGlossaryRepository _glossaryRepository;
    private readonly IGlossaryLocalizationRepository _glossaryLocalizationRepository;
    private readonly ILanguageRepository _languageRepository;

    public GetGlossaryForAdminQueryHandler(ILogger<GetGlossaryForAdminQueryHandler> logger,
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

    public async Task<GetGlossaryForAdminQueryDto> Handle(GetGlossaryForAdminQuery request, CancellationToken cancellationToken)
    {
        // get the glossary by its id
        var glossary = await _glossaryRepository.GetByIdAsync(request.GlossaryId);
        // map the glossary
        var glossaryDto = _mapper.Map<GetGlossaryForAdminQueryDto>(glossary);
        //fill localization for the Glossary
        await LocalizationHelper.FillGlossaryLocalizations(glossaryDto,_glossaryLocalizationRepository,_mapper);
        // fill key and language name for the Glossary localization
        foreach (var localization in glossaryDto.GlossaryLocalizationDtos)
        {
            localization.LanguageName = _languageRepository.GetByIdAsync(localization.LanguageId).Result.Name;
            localization.Key = _glossaryRepository.GetByIdAsync(localization.GlossaryId).Result.Key;
        }
        return glossaryDto;
    }
}
