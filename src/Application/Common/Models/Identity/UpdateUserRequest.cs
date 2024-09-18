using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Assets;
using Offers.CleanArchitecture.Application.Common.Models.Enums;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public class UpdateUserRequest
{
    public string UserId { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public Guid CountryId { get; set; }
    public Guid LanguageId { get; set; }
    public FileDto? File { get; set; }
    public JobRole JobRole { get; set; }
    public List<string> GroupIds { get; set; } = new List<string>();
    public List<string> NotificationGroupIds { get; set; } = new List<string>();

}
