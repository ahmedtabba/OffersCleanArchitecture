namespace Offers.CleanArchitecture.Api.NeededDto.Language;

public class UpdateLanguageCommandDto
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool RTL { get; set; } // null value of bool is false
}
