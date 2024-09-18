using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Offers.CleanArchitecture.Application.Common.Models.Identity;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class ApplicationRole : IdentityRole,IApplicationRole
{
    public ApplicationRole()
    {

    }

    /// <summary>
    /// That description field used as friendly name 
    /// </summary>
    public string Description { get; set; } = null!;
    public virtual ICollection<ApplicationGroup> Groups { get; set; } = new List<ApplicationGroup>();
    [NotMapped]
    public ICollection<ApplicationGroupHelper> GroupRoles { get; set; } = new List<ApplicationGroupHelper>();

    public override string ToString()
    {
        return this.Name!;
    }
}
