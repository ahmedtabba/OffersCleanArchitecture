using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Offers.CleanArchitecture.Application.Common.GenericExtensions;
using Offers.CleanArchitecture.Application.Common.Interfaces.Identity;
using Offers.CleanArchitecture.Application.Common.Interfaces.IRepositories;
using Offers.CleanArchitecture.Application.Common.Models;
using Offers.CleanArchitecture.Application.Groceries.Commands.DeleteGrocery;
using Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceriesWithPagination;
using Offers.CleanArchitecture.Application.Posts.EventHandlers;
using Offers.CleanArchitecture.Application.Posts.Queries.GetPostsByGroceryWithPagination;
using Offers.CleanArchitecture.Application.Utilities;
using Offers.CleanArchitecture.Domain.Entities;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Groceries.Queries.GetGroceryQuery;
public class GetGroceryQuery : IRequest<GetGroceryDto>
{
    public Guid GroceryId { get; set; }
    public Guid CountryId { get; set;}
    public Guid LanguageId { get; set; }

}

public class GetGroceryQueryHandler : IRequestHandler<GetGroceryQuery, GetGroceryDto>
{
    private readonly IGroceryRepository _groceryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetGroceryQueryHandler> _logger;
    private readonly IUserContext _userContext;
    private readonly IGroceryLocalizationRepository _groceryLocalizationRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IFavoraiteGroceryRepository _favoraiteGroceryRepository;
    private readonly IUser _user;

    public GetGroceryQueryHandler(IGroceryRepository groceryRepository,
                                  IMapper mapper,
                                  ILogger<GetGroceryQueryHandler> logger,
                                  IUserContext userContext,
                                  IGroceryLocalizationRepository groceryLocalizationRepository,
                                  ICountryRepository countryRepository,
                                  IFavoraiteGroceryRepository favoraiteGroceryRepository,
                                  IUser user)
    {
        _groceryRepository = groceryRepository;
        _mapper = mapper;
        _logger = logger;
        _userContext = userContext;
        _groceryLocalizationRepository = groceryLocalizationRepository;
        _countryRepository = countryRepository;
        _favoraiteGroceryRepository = favoraiteGroceryRepository;
        _user = user;
    }
    public async Task<GetGroceryDto> Handle(GetGroceryQuery request, CancellationToken cancellationToken)
    {
        // grocery here has posts so mapper will return GroceryDto with posts
        //var grocery = await _groceryRepository.GetGroceryWithPostsByGroceryId(request.GroceryId);// if we need posts of the grocery 
        var grocery = await _groceryRepository.GetByIdAsync(request.GroceryId);

        var groceryDto = _mapper.Map<GetGroceryDto>(grocery);

        // if LanguageId is null (Guid.Empty) we will return standard
        if (request.LanguageId != Guid.Empty)
        {
            var groceryLocalizations = await _groceryLocalizationRepository.GetAll()
                .Where(gl => gl.GroceryId == groceryDto.Id && gl.LanguageId == request.LanguageId)
                .ToListAsync();
            if (groceryLocalizations.Count>0)
            {
                groceryDto.Name = groceryLocalizations.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Name) != null
                ? groceryLocalizations.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Name)!.Value
                : groceryDto.Name;
                groceryDto.Description = groceryLocalizations.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Description) != null
                    ? groceryLocalizations.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Description)!.Value
                    : groceryDto.Description;
                groceryDto.Address = groceryLocalizations.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Address) != null
                    ? groceryLocalizations.FirstOrDefault(gl => gl.GroceryLocalizationFieldType == (int)GroceryLocalizationFieldType.Address)!.Value
                    : groceryDto.Address;
            }
        }

        // check if user is authorized or if grocery is favorite for the user by its token 
        if(_userContext.CheckIfUserAuthorized() && await _favoraiteGroceryRepository.GetAll().AnyAsync(f => f.UserId == _user.Id && f.GroceryId == groceryDto.Id))
            groceryDto.IsFavorite = true;
        else 
            groceryDto.IsFavorite = false;

        groceryDto.CountryName = await _countryRepository.GetCountryNameByIdAsync(groceryDto.CountryId);
        return groceryDto;
    }
}
