using System;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    //public abstract class BaseEvent
    //{

    //}

    public class AnswerCreatedEvent : IEvent
    {
        public AnswerCreatedEvent(Answer answer)
        {
            Answer = answer;
        }

        public Answer Answer { get; private set; }
    }
}