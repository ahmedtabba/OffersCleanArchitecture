using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class Post : BaseAuditableEntity
{
    public Post() : base()
    {

    }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
    public string ImagePath { get; set; } = null!;
    public Guid GroceryId { get; set; }
    public virtual Grocery Grocery { get; set; } = new Grocery();

    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    public virtual ICollection<PostLocalization> PostsLocalization { get; set; } = new List<PostLocalization>();

    //public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    //public virtual ICollection<LanguagePost> LanguagePosts { get; set; } = new List<LanguagePost>();
    public DateTime? PublishDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
