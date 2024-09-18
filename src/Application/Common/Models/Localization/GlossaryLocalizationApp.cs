using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Application.Common.Models.Localization;
public class GlossaryLocalizationApp
{
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = null!;
}
