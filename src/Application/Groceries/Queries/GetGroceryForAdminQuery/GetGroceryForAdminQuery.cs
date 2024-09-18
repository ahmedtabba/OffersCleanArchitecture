using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Utilities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
public class GetGroceryForAdminQuery : IRequest<GetGroceryForAdminDto>
{
    public Guid GroceryId { get; set; }
}

public class GetGroceryForAdminQueryHandler : IRequestHandler<GetGroceryForAdminQuery, GetGroceryForAdminDto>
{
    private readonly ILogger<GetGroceryForAdminQueryHandler> _logger;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IMapper _mapper;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;
    private readonly ICountryRepository _countryRepository;

    public GetGroceryForAdminQueryHandler(ILogger<GetGroceryForAdminQueryHandler> logger,
                                          IGroceryRepository groceryRepository,
                                          IMapper mapper,
                                          IGroceryLocalizationRepository groceryLocalizationRepository,
                                          ICountryRepository countryRepository )
    {
        _logger = logger;
        _groceryRepository = groceryRepository;
        _mapper = mapper;
        _groceryLocalizationRepository = groceryLocalizationRepository;
        _countryRepository = countryRepository;
    }

    public async Task<GetGroceryForAdminDto> Handle(GetGroceryForAdminQuery request, CancellationToken cancellationToken)
    {
        var grocery = await _groceryRepository.GetByIdAsync(request.GroceryId);//without posts
        var groceryWithFullLocalizationDto = _mapper.Map<GetGroceryForAdminDto>(grocery);
        await LocalizationHelper.FillGroceryLocalizations(groceryWithFullLocalizationDto, _groceryLocalizationRepository, _mapper);
        groceryWithFullLocalizationDto.CountryName = await _countryRepository.GetCountryNameByIdAsync(groceryWithFullLocalizationDto.CountryId);
        return groceryWithFullLocalizationDto;
    }
}
