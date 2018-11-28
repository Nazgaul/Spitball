using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Event
{
    class UserUnSuspendEvent : IEvent
    {
        public UserUnSuspendEvent(User user)
        {
            User = user;
        }
        public User User { get; private set; }
    }
}
