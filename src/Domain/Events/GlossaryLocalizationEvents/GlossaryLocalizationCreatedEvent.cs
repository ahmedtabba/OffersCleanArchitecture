using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.GlossaryLocalizationEvents;
public class GlossaryLocalizationCreatedEvent : BaseEvent
{
    public GlossaryLocalizationCreatedEvent(GlossaryLocalization glossaryLocalization)
    {
        GlossaryLocalization = glossaryLocalization;
    }
    public GlossaryLocalization GlossaryLocalization { get; }
}
