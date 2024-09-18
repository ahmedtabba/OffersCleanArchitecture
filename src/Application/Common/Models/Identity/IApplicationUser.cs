using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;
using System.Text.Json.Serialization;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public interface IApplicationUser
{
    public string Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    ////
    public bool HasPhoto { get; set; }
    public JobRole JobRole { get; set; }
    public string FullName { get; set; }
    public string PhotoURL { get; set; }
    public string CountryId { get; set; }
    public string CountryName { get; set; }
    public string LanguageId { get; set; }
    public string LanguageName { get; set; }
    public string? TokenVersion {  get; set; }
    public List<string>? NotificationGroupIds { get; set; } 
    public List<string>? UserNotificationIds { get; set; }

    //public ICollection<IApplicationUserGroup> Groups { get; set; }
    //[JsonPropertyName("userGroups")]
    public ICollection<ApplicationGroupHelper> UserGroups { get; set; }
}

