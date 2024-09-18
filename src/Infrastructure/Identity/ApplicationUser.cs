using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Offers.CleanArchitecture.Infrastructure.Identity;

public class ApplicationUser : IdentityUser, IApplicationUser
{
    public bool HasPhoto { get; set; }
    public JobRole JobRole { get; set; }
    public string FullName { get; set; } = null!;
    public string PhotoURL { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public string LanguageId { get; set; } = null!;
    public virtual ICollection<ApplicationGroup> Groups { get; set; } = new List<ApplicationGroup>();

    [NotMapped]
    public ICollection<Offers.CleanArchitecture.Application.Identity.HelperClasses.ApplicationGroupHelper> UserGroups { get; set; } =
        new List<Offers.CleanArchitecture.Application.Identity.HelperClasses.ApplicationGroupHelper>();
    [NotMapped]
    public string CountryName { get; set; } = null!;
    [NotMapped]
    public string LanguageName { get; set; } = null!;
    public string? TokenVersion { get; set; }
    public List<string>? NotificationGroupIds { get ; set; } = new List<string>();
    public List<string>? UserNotificationIds { get; set; } = new List<string>();
}
