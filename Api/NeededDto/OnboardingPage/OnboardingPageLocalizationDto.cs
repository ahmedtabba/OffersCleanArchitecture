namespace Offers.CleanArchitecture.Api.NeededDto.OnboardingPage;

public class OnboardingPageLocalizationDto
{
    public Guid LanguageId { get; set; }
    public int FieldType { get; set; }
    public string Value { get; set; } = null!;
}
