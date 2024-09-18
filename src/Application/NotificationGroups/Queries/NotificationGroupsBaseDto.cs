using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.NotificationGroups.Queries;
public class NotificationGroupsBaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
