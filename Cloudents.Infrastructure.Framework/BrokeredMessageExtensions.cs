using System;
using Microsoft.ServiceBus.Messaging;

namespace Cloudents.Infrastructure.Framework
{
    public static class BrokeredMessageExtensions
    {
        public static T GetBodyInheritance<T>(this BrokeredMessage message) where T : class
        {
            if (message.Properties.TryGetValue("messageType", out var messageType))
            {
                var messageBodyType = Type.GetType(messageType.ToString());
                if (messageBodyType == null)
                {
                    //Should never get here as a messagebodytype should
                    //always be set BEFORE putting the message on the queue
                    message.DeadLetter();
                }

                //read body only if event handler hooked
                var method = typeof(BrokeredMessage).GetMethod("GetBody", Array.Empty<Type>()) ?? throw new NullReferenceException("no method GetBody");
                var generic = method.MakeGenericMethod(messageBodyType);
                var messageBody = generic.Invoke(message, null);
                return (T)messageBody;
            }
            message.DeadLetter();
            return null;
        }
    }
}