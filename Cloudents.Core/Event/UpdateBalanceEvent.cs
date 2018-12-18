using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Event
{
    public class UpdateBalanceEvent : IEvent
    {
        public UpdateBalanceEvent(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}
