

namespace Offers.CleanArchitecture.Api.NeededDto.OnboardingPage;

public class CreateOnboardingPageCommandDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IFormFile Asset { get; set; } = null!;
    public int Order {  get; set; }
    public OnboardingPageLocalizationDto[] OnboardingPageLocalizationDtos { get; set; } = [];
    public OnboardingPageLocalizationAssetsDto[] LocalizedAssets { get; set; } = [];

}
