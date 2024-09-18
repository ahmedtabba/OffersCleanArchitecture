using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.OnboardingPages.Queries;
public class OnboardingPageLocalizationBaseDto
{
    public Guid Id { get; set; }
    public Guid LanguageId { get; set; }
    public Guid OnboardingPageId { get; set; }
    public int OnboardingPageLocalizationFieldType { get; set; }
    public string Value { get; set; } = null!;
}
