using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public interface  IApplicationUserGroup
{
    public string Id { get; set; }
    public string ApplicationUserId { get; set; }
    public string ApplicationGroupId { get; set; }
    //public IApplicationGroup ApplicationGroup { get; set; }
}


