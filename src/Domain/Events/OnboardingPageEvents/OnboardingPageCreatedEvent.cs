using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.OnboardingPageEvents;
public class OnboardingPageCreatedEvent : BaseEvent
{
    public OnboardingPageCreatedEvent(OnboardingPage onboardingPage)
    {
        OnboardingPage = onboardingPage;
    }
    public OnboardingPage OnboardingPage { get; }
}
