using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;

namespace Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;
public class GetCountriesWithPaginationQuery : IRequest<PaginatedList<GetCountriesWithPaginationDto>> 
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public string? Sort { get; set; }
}

public class GetCountriesWithPaginationHandler : IRequestHandler<GetCountriesWithPaginationQuery, PaginatedList<GetCountriesWithPaginationDto>>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<GetCountriesWithPaginationHandler> _logger;
    private readonly IMapper _mapper;

    public GetCountriesWithPaginationHandler(ICountryRepository countryRepository,
                                             ILogger<GetCountriesWithPaginationHandler> logger,
                                             IMapper mapper)
    {
        _countryRepository = countryRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<PaginatedList<GetCountriesWithPaginationDto>> Handle(GetCountriesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var countries = _countryRepository.GetAll();
        if (request.SearchText != null)
            countries = countries.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower()) /*|| x.Code.ToLower().Contains(request.SearchText.ToLower())*/);

        var result = await countries
            .Order(request.Sort)
            .ProjectTo<GetCountriesWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return result;
    }
}
