namespace Offers.CleanArchitecture.Api.NeededDto.Post;

public class PostLocalizationAssetsDto
{
    public Guid LanguageId { get; set; }
    public IFormFile Asset { get; set; } = null!;
}
