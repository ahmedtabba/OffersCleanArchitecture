using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;

namespace Offers.CleanArchitecture.Application.Identity.Queries.GetUsersWithPagination;
public class UserDto // not used now
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }
    public string? FullName { get; set; }

    public string? NormalizedUserName { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool HasPhoto { get; set; }
    public string? PhotoURL { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public IEnumerable<ApplicationGroupHelper> GroupHelper { get; set; } = new List<ApplicationGroupHelper>();

}
