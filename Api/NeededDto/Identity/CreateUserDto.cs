using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Enums;

namespace Offers.CleanArchitecture.Api.NeededDto.Identity;

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public IFormFile? File { get; set; }
    public int JobRole { get; set; }
    public List<string> GroupIds { get; set; } = new List<string>();
    public List<Guid> NotificationGroupIds { get; set; } = new List<Guid>();
}
