using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.LanguageEvents;
public class LanguageCreatedEvent : BaseEvent
{
    public LanguageCreatedEvent(Language language)
    {
        Language = language;
    }
    public Language Language { get; }
}
