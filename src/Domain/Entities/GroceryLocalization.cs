using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class GroceryLocalization : BaseAuditableEntity
{
    public GroceryLocalization() : base()
    {

    }
    public Guid LanguageId { get; set; }
    public Guid GroceryId { get; set; }
    //public virtual Language Language { get; set; } = null!;
    //public virtual Grocery Grocery { get; set; } = null!;
    public int GroceryLocalizationFieldType { get; set; }
    public string Value { get; set; } = null!;
}
