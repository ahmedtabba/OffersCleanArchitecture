using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Resources;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;

namespace Offers.CleanArchitecture.Application.Languages.Queries.GetLanguagesWithPagination;
public class GetLanguagesWithPaginationQuary : IRequest<PaginatedList<GetLanguagesWithPaginationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
}

public class GetLanguagesWithPaginationQuaryHandler : IRequestHandler<GetLanguagesWithPaginationQuary, PaginatedList<GetLanguagesWithPaginationDto>>
{
    private readonly ILogger<GetLanguagesWithPaginationQuaryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly ILanguageRepository _languageRepository;

    public GetLanguagesWithPaginationQuaryHandler(ILogger<GetLanguagesWithPaginationQuaryHandler> logger,
                                                  IMapper mapper,
                                                  ILanguageRepository languageRepository )
    {
        _logger = logger;
        _mapper = mapper;
        _languageRepository = languageRepository;
    }

    public async Task<PaginatedList<GetLanguagesWithPaginationDto>> Handle(GetLanguagesWithPaginationQuary request, CancellationToken cancellationToken)
    {
        var languages = _languageRepository.GetAll();
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            languages = languages.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower()) || x.Code.ToLower().Contains(request.SearchText.ToLower()));

        var result = await languages
            .OrderBy(l => l.Name)
            .ProjectTo<GetLanguagesWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}
