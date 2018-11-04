using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Event
{
    class SuspendUserEvent : IEvent
    {
        public SuspendUserEvent(User _user)
        {
            User = _user;
        }
        public User User { get; private set; }
    }
}
