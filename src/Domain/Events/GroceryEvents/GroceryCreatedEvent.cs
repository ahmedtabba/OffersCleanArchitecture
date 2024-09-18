using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.GroceryEvents;
public class GroceryCreatedEvent : BaseEvent
{
    public GroceryCreatedEvent(Grocery grocery)
    {
        Grocery = grocery;
    }
    public Grocery Grocery { get; }
}
