using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Identity.HelperClasses;

namespace Offers.CleanArchitecture.Application.Common.Models.Identity;
public interface IApplicationRole 
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? NormalizedName { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string Description { get; set; }
    [JsonPropertyName("groupRoles")]
    public ICollection<ApplicationGroupHelper> GroupRoles { get; set; }
}
