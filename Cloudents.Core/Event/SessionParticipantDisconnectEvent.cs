using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Event
{
    public class SessionParticipantDisconnectEvent : IEvent
    {
        public SessionParticipantDisconnectEvent(SessionParticipantDisconnect sessionDisconnect)
        {
            SessionDisconnect = sessionDisconnect;
        }
        public SessionParticipantDisconnect SessionDisconnect { get; private set; }
    }
}
