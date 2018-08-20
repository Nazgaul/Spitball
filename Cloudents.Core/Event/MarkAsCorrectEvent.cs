using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class MarkAsCorrectEvent : IEventMessage
    {
        public MarkAsCorrectEvent(Guid answerId)
        {
            AnswerId = answerId;
        }

        public Guid AnswerId { get; set; }

    }
}