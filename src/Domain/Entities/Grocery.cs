using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class Grocery : BaseAuditableEntity
{
    public Grocery():base()
    {
        
    }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string LogoPath { get; set; } = null!;
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<FavoraiteGrocery> FavoraiteGroceries { get; set; } = new List<FavoraiteGrocery>();

    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();

    public virtual ICollection<GroceryLocalization> GroceriesLocalization { get; set; } = new List<GroceryLocalization>();

    //public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    //public virtual ICollection<LanguageGrocery> LanguageGroceries { get; set; } = new List<LanguageGrocery>();
    public Guid CountryId { get; set; }
    public virtual Country Country { get; set; } = new Country();




}
