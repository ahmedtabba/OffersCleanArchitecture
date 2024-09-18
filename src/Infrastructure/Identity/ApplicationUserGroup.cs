using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class ApplicationUserGroup : IApplicationUserGroup
{
    public ApplicationUserGroup()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    public string ApplicationGroupId { get; set; }=null!;
    //public ApplicationGroup ApplicationGroup { get; set; }
    //public ApplicationUser ApplicationUser { get; set; }
}
