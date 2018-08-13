using System;

namespace Cloudents.Core.Event
{
    public class MarkAsCorrectEvent
    {
        public MarkAsCorrectEvent(Guid answerId)
        {
            AnswerId = answerId;
        }

        public Guid AnswerId { get; set; }

    }
}