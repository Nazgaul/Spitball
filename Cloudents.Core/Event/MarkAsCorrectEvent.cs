using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class MarkAsCorrectEvent : IEvent
    {
        public MarkAsCorrectEvent(Guid answerId)
        {
            AnswerId = answerId;
        }

        public Guid AnswerId { get; set; }

    }
}