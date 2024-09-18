using Offers.CleanArchitecture.Application.Common.Models.Assets;

namespace Offers.CleanArchitecture.Api.NeededDto.Grocery;

public class CreateGroceryCommandDto
{
    // Represent needed information to create Grocery
    public Guid CountryId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public IFormFile Logo { get; set; } = null!;

    public GroceryLocalizationDto[] GroceryLocalizationDtos { get; set; } = [];
}
