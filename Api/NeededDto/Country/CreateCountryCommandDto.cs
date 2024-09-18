using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Offers.CleanArchitecture.Api.NeededDto.Country;

public class CreateCountryCommandDto
{
    public string Name { get; set; } = null!;
    public IFormFile Flag { get; set; } = null!;
    public string TimeZoneId { get; set; } = null!;
    public string Code { get; set; } = null!;
}
