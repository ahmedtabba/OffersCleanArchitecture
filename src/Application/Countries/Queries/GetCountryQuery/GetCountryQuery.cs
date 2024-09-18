using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Countries.Queries.GetCountriesWithPagination;

namespace Offers.CleanArchitecture.Application.Countries.Queries.GetCountryQuery;
public class GetCountryQuery : IRequest<GetCountryDto>
{
    public Guid CountryId { get; set; }
}

public class GetCountryQueryHandler : IRequestHandler<GetCountryQuery, GetCountryDto>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger<GetCountryQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetCountryQueryHandler(ICountryRepository countryRepository,
                                  ILogger<GetCountryQueryHandler> logger,
                                  IMapper mapper)
    {
        _countryRepository = countryRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<GetCountryDto> Handle(GetCountryQuery request, CancellationToken cancellationToken)
    {
        var country = await _countryRepository.GetByIdAsync(request.CountryId);
        var countryDto =  _mapper.Map<GetCountryDto>(country);
        return countryDto;
    }
}
