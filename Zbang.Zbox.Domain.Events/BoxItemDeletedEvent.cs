using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Events;

namespace Zbang.Zbox.Domain.Events
{
    public class BoxItemDeletedEvent : BaseEvent
    {
        readonly Type m_boxItem;
        private readonly string m_BoxItemName;

        public BoxItemDeletedEvent(string emailId, long boxId, Type boxItem, string boxItemName)
            : base(emailId, boxId)
        {
            this.m_boxItem = boxItem;
            m_BoxItemName = boxItemName;
        }

        public string boxItemName
        {
            get { return m_BoxItemName; }
          
        }
        

        public string boxItem
        {
            get
            {
                return m_boxItem.Name;
            }
        }
    }
}
