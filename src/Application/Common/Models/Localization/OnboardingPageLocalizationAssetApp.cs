using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.CleanArchitecture.Application.Common.Models.Assets;

namespace Offers.CleanArchitecture.Application.Common.Models.Localization;
public class OnboardingPageLocalizationAssetApp
{
    public Guid LanguageId { get; set; }
    public FileDto Asset { get; set; } = null!; 
}
