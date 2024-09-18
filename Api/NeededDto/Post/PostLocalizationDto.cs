using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Api.NeededDto.Grocery;

public class PostLocalizationDto
{
    public Guid LanguageId { get; set; }
    public int FieldType { get; set; }
    public string Value { get; set; } = null!;
}
