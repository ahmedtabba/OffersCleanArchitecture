using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class ApplicationGroup : IApplicationGroup
{
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; } = new List<ApplicationRole>();

    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    [NotMapped]
    public ICollection<ApplicationRoleHelper> ApplicationRolesHelper { get; set; } = new List<ApplicationRoleHelper>();
    [NotMapped]
    public ICollection<ApplicationUserHelper> ApplicationUsersHelper { get; set; } = new List<ApplicationUserHelper>();

    public ApplicationGroup()
    {
        this.Id = Guid.NewGuid().ToString();
    }
}
