using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class PostLocalization : BaseAuditableEntity
{
    public PostLocalization() : base()
    {

    }
    public Guid LanguageId { get; set; }
    public Guid PostId { get; set; }
    //public virtual Language Language { get; set; } = null!;
    //public virtual Post Post { get; set; } = null!;
    public int PostLocalizationFieldType { get; set; }
    public string Value { get; set; } = null!;
}
