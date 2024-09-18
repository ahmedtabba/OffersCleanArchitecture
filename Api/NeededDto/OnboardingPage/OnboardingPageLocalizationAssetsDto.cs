namespace Offers.CleanArchitecture.Api.NeededDto.OnboardingPage;

public class OnboardingPageLocalizationAssetsDto
{
    public Guid LanguageId { get; set; }
    public IFormFile Asset { get; set; } = null!;
}
