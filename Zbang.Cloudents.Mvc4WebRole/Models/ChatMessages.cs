using System;
using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ChatMessages
    {
        private DateTime? m_DateTime;
        public Guid? ChatRoom { get; set; }
        public IEnumerable<long> UserIds { get; set; }

        public DateTime? StartTime
        {
            get
            {
                return m_DateTime;
            }
            set
            {
                if (value.HasValue)
                {
                    m_DateTime = value.Value.ToUniversalTime();
                }
                else
                {
                    m_DateTime = value;
                }
            }
        }

        public int Top { get; set; }
    }
}