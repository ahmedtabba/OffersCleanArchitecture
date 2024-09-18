namespace Offers.CleanArchitecture.Api.NeededDto.Glossary;

public class UpdateGlossaryCommandDto
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public GlossaryLocalizationDto[] Localization { get; set; } = [];

}
