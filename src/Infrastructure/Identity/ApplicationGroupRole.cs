using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Identity;

namespace Offers.CleanArchitecture.Infrastructure.Identity;
public class ApplicationGroupRole : IApplicationGroupRole
{
    public string Id { get; set; }
    public string ApplicationGroupId { get; set; } = null!;
    public string ApplicationRoleId { get; set; } = null!;
    //public ApplicationGroup ApplicationGroup { get; set; }

    public ApplicationGroupRole()
    {
        Id = Guid.NewGuid().ToString();
    }
}
