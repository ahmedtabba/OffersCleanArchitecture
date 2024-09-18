using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.OnboardingPageLocalizationEvents;
public class OnboardingPageLocalizationCreatedEvent : BaseEvent
{
    public OnboardingPageLocalizationCreatedEvent(OnboardingPageLocalization onboardingPageLocalization)
    {
        OnboardingPageLocalization = onboardingPageLocalization;
    }
    public OnboardingPageLocalization OnboardingPageLocalization { get; }
}
