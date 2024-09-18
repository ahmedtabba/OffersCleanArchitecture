using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class Country : BaseAuditableEntity
{
    public Country() : base() 
    {

    }
    public string Name { get; set; } = null!;
    public string? Code { get; set; }
    public string? FlagPath { get; set; } = null!;
    public string? TimeZoneId { get; set; } = null!;
    public virtual ICollection<Grocery> Groceries { get; set; } = new List<Grocery>();
    //public List<Guid> UsersIds { get; set; } = new List<Guid>();// not useful

}
