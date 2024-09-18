using Offers.CleanArchitecture.Api.NeededDto.Grocery;
using Offers.CleanArchitecture.Domain.Entities;

namespace Offers.CleanArchitecture.Api.NeededDto.Post;

public class CreatePostCommandDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool? IsActive { get; set; }
    public IFormFile Asset { get; set; } = null!;
    public DateTime? PublishDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PostLocalizationDto[] PostLocalizationDtos { get; set; } = [];
    public PostLocalizationAssetsDto[] LocalizedAssets { get; set; } = [];

}
