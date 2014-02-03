using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Domain.Events
{
    public class BoxSubscriberAddedEvent : BaseEvent
    {
        public BoxSubscriberAddedEvent(string emailId, long boxId)
            : base(emailId, boxId)
        {

        }
    }
}
