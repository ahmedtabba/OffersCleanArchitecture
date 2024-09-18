using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Interfaces.Services;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryForAdminQuery;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesForAdminWithPagination;
public class GetGroceriesForAdminWithPaginationQuery : IRequest<PaginatedList<GetGroceriesForAdminWithPaginationDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; set; }
    public string? Sort { get; set; }
}

public class GetGroceriesForAdminWithPaginationQueryHandler : IRequestHandler<GetGroceriesForAdminWithPaginationQuery, PaginatedList<GetGroceriesForAdminWithPaginationDto>>
{
    private readonly ILogger<GetGroceriesForAdminWithPaginationQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IGroceryRepository _groceryRepository;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;
    private readonly IUserContext _userContext;
    private readonly ICacheService _cacheService;

    public GetGroceriesForAdminWithPaginationQueryHandler(ILogger<GetGroceriesForAdminWithPaginationQueryHandler> logger,
                                                          IMapper mapper,
                                                          IGroceryRepository groceryRepository,
                                                          IGroceryLocalizationRepository groceryLocalizationRepository,
                                                          IUserContext userContext,
                                                          ICacheService cacheService)
    {
        _logger = logger;
        _mapper = mapper;
        _groceryRepository = groceryRepository;
        _groceryLocalizationRepository = groceryLocalizationRepository;
        _userContext = userContext;
        _cacheService = cacheService;
    }

    public async Task<PaginatedList<GetGroceriesForAdminWithPaginationDto>> Handle(GetGroceriesForAdminWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // we return only groceries in the same country of admin
        IQueryable<Grocery> groceries = _groceryRepository.GetAllByCountryId(_userContext.GetCountryIdOfUser());

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            groceries = groceries.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower()) || x.Address.ToLower().Contains(request.SearchText.ToLower()));

        var result = await groceries
            .Order(request.Sort)
            .ProjectTo<GetGroceriesForAdminWithPaginationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        // fill localization of the grocery
        await LocalizationHelper.FillGroceryLocalizations(result, _groceryLocalizationRepository, _mapper);
        return result;
   
    }
}
