using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Entities;
public class OnboardingPageLocalization : BaseAuditableEntity
{
    public OnboardingPageLocalization() : base()
    {
        
    }
    public Guid LanguageId { get; set; }
    public Guid OnboardingPageId { get; set; }
    public int OnboardingPageLocalizationFieldType { get; set; }
    public string Value { get; set; } = null!;
}
