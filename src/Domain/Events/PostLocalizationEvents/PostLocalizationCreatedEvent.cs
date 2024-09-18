using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.PostLocalizationEvents;
public class PostLocalizationCreatedEvent : BaseEvent
{
    public PostLocalizationCreatedEvent(PostLocalization postLocalization)
    {
        PostLocalization = postLocalization;
    }
    public PostLocalization PostLocalization { get; }
}
