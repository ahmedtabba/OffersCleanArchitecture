using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class Language : BaseAuditableEntity
{
    public Language() : base()
    {

    }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public bool RTL { get; set; } = false;
    public virtual ICollection<Grocery> Groceries { get; set; } = new List<Grocery>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<GroceryLocalization> GroceriesLocalization { get; set; } = new List<GroceryLocalization>();
    public virtual ICollection<PostLocalization> PostsLocalization { get; set; } = new List<PostLocalization>();
    public virtual ICollection<OnboardingPage> OnboardingPages { get; set;} = new List<OnboardingPage>();
    public virtual ICollection<Glossary> Glossaries { get; set; } = new List<Glossary>();
    public virtual ICollection<GlossaryLocalization>  GlossariesLocalization { get; set; } = new List<GlossaryLocalization>();

    //public virtual ICollection<Grocery> Groceries { get; set; } = new List<Grocery>();
    //public virtual ICollection<LanguagePost> LanguagePosts { get; set; } = new List<LanguagePost>();


    //public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    //public virtual ICollection<LanguageGrocery> LanguageGroceries { get; set; } = new List<LanguageGrocery>();
}
