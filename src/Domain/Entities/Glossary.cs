using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class Glossary : BaseAuditableEntity
{
    public Glossary() : base()
    {

    }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    public virtual ICollection<GlossaryLocalization> GlossariesLocalization { get; set;} = new List<GlossaryLocalization>();
    
}
