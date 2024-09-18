using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Enums;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public class CreateUserRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public FileDto? File { get; set; }
    public JobRole JobRole { get; set; }
    public List<string> GroupIds { get; set; } = new List<string>();
    public List<string> NotificationGroupIds { get; set; } = new List<string>();
}
