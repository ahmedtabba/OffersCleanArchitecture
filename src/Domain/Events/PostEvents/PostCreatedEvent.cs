using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.CleanArchitecture.Domain.Events.PostEvents;
public class PostCreatedEvent : BaseEvent
{
    public PostCreatedEvent(Post post)
    {
        Post = post;
    }
    public Post Post { get; }
}
