using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Domain.Enums;

namespace Offers.CleanArchitecture.Application.Common.Models.Localization;
public class OnboardingPageLocalizationApp
{
    public Guid LanguageId { get; set; }
    public OnboardingPageLocalizationFieldType FieldType { get; set; }
    public string Value { get; set; } = null!;
}
