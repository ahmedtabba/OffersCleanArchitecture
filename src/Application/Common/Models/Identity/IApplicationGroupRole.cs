using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public interface IApplicationGroupRole
{
    public string Id { get; set; }
    public string ApplicationGroupId { get; set; }
    public string ApplicationRoleId { get; set; }
}
