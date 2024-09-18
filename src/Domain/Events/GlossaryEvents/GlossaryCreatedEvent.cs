using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.GlossaryEvents;
public class GlossaryCreatedEvent : BaseEvent
{
    public GlossaryCreatedEvent(Glossary glossary)
    {
        Glossary = glossary;
    }
    public Glossary Glossary { get; }
}
