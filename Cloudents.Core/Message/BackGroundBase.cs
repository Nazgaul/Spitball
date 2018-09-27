using System;
using Cloudents.Core.Storage;

namespace Cloudents.Core.Message
{
    [Serializable]
    public abstract class ServiceBusMessageBase
    {
        protected ServiceBusMessageBase(TopicSubscription topicSubscription)
        {
            TopicSubscription = topicSubscription;
        }

        public TopicSubscription TopicSubscription { get; private set; }
    }
}