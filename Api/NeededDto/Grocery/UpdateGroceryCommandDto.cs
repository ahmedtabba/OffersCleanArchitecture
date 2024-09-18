namespace Offers.CleanArchitecture.Api.NeededDto.Grocery;

public class UpdateGroceryCommandDto
{
    // Represent information to be updated for Grocery
    public Guid CountryId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public IFormFile? Logo { get; set; } // Represent new logo of the Grocery, and may be null if no need to change it

    public GroceryLocalizationDto[] GroceryLocalizationDtos { get; set; } = [];

}
