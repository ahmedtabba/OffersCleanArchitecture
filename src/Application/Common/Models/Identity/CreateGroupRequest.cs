using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public class CreateGroupRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
