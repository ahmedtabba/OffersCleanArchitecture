using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Mappings;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
public class GetGroceriesWithPaginationQuery : IRequest<PaginatedList<GetGroceriesWithPaginationDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchText { get; set; }
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
}

public class GetGroceriesWithPaginationQueryHandler : IRequestHandler<GetGroceriesWithPaginationQuery, PaginatedList<GetGroceriesWithPaginationDto>>
{
    private readonly IMapper _mapper;
    private readonly IGroceryRepository _groceryRepository;
    private readonly ILogger<GetGroceriesWithPaginationQueryHandler> _logger;
    private readonly ICountryRepository _countryRepository;
    private readonly IUserContext _userContext;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;

    public GetGroceriesWithPaginationQueryHandler(IMapper mapper,
                                                  IGroceryRepository groceryRepository,
                                                  ILogger<GetGroceriesWithPaginationQueryHandler> logger,
                                                  ICountryRepository countryRepository,
                                                  IUserContext userContext,
                                                  IGroceryLocalizationRepository groceryLocalizationRepository,
                                                  IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                                  IUser user)
    {
        _mapper = mapper;
        _groceryRepository = groceryRepository;
        _logger = logger;
        _countryRepository = countryRepository;
        _userContext = userContext;
        _groceryLocalizationRepository = groceryLocalizationRepository;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _user = user;
    }
    public async Task<PaginatedList<GetGroceriesWithPaginationDto>> Handle(GetGroceriesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // we return only groceries in the same country of user
        var groceries = _groceryRepository.GetAll()
            .Where(g => g.CountryId == request.CountryId);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
            groceries = groceries.Where(x => x.Name.ToLower().Contains(request.SearchText.ToLower()) || x.Address.ToLower().Contains(request.SearchText.ToLower()));

        var result = await groceries
            .OrderBy(g => g.Name)
            //.OrderBy(g => g.CountryId)
            .ProjectTo<GetGroceriesWithPaginationDto>(_mapper.ConfigurationProvider)// groceries here don't have posts
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        var countryName =  await _countryRepository.GetCountryNameByIdAsync(request.CountryId);
        foreach (var groceryDto in result.Items)
        {
            // fill localization according to user language
            if (request.LanguageId != Guid.Empty)
            {
                var groceryLocalization = await _groceryLocalizationRepository.GetAll()
                .Where(gl => gl.GroceryId == groceryDto.Id && gl.LanguageId == request.LanguageId).ToListAsync();
                if (groceryLocalization.Count > 0)
                {
                    groceryDto.Name = groceryLocalization.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Name) != null
                    ? groceryLocalization.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Name).Value
                    : groceryDto.Name;
                    groceryDto.Description = groceryLocalization.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Description) != null
                        ? groceryLocalization.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Description).Value
                        : groceryDto.Description;
                    groceryDto.Address = groceryLocalization.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Address) != null
                        ? groceryLocalization.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Address).Value
                        : groceryDto.Address;
                }
            }
            // check if user is authorized or if grocery is favorite for the user by its token
            if (_userContext.CheckIfUserAuthorized() && await _favoraiteGroceryRepository.GetAll().AnyAsync(f => f.UserId == _user.Id && f.GroceryId == groceryDto.Id))
                groceryDto.IsFavorite = true;
            else
                groceryDto.IsFavorite = false;

            groceryDto.CountryName = countryName;

        }
        return result;
        }
}
