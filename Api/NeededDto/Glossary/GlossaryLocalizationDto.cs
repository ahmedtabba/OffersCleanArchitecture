namespace Offers.CleanArchitecture.Api.NeededDto.Glossary;

public class GlossaryLocalizationDto
{
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = null!;
}
