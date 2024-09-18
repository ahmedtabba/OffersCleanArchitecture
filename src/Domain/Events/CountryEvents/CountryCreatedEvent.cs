using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.CountryEvents;
public class CountryCreatedEvent : BaseEvent
{
    public CountryCreatedEvent(Country country)
    {
        Country = country;
    }
    public Country Country { get; }
}
