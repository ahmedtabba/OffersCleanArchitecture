using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class OnboardingPage : BaseAuditableEntity
{
    public OnboardingPage() : base()
    {
        
    }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string AssetPath { get; set; } = null!;
    public int Order {  get; set; }
    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
}
