using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.GroceryLocalizationEvents;
public class GroceryLocalizationCreatedEvent : BaseEvent
{
    public GroceryLocalizationCreatedEvent(GroceryLocalization groceryLocalization)
    {
        GroceryLocalization = groceryLocalization;
    }
    public GroceryLocalization GroceryLocalization { get; }
}
