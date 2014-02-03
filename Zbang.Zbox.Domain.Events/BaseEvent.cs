using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Events;

namespace Zbang.Zbox.Domain.Events
{
    public abstract class BaseEvent : IEvent
    {
        //Fields

        private readonly long m_BoxId;

        //Ctor
        public BaseEvent(string emailId, long boxId)
        {
            EmailId = emailId;
            m_BoxId = boxId;
        }

        //Properties
        public string EmailId
        {
            get;
            private set;
        }
        public long BoxId
        {
            get
            {
                return m_BoxId;
            }
        }
    }
}
