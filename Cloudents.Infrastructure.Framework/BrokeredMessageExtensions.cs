using System;
using Microsoft.ServiceBus.Messaging;

namespace Cloudents.Infrastructure.Framework
{
    public static class BrokeredMessageExtensions
    {
        public static T GetBodyInheritance<T>(this BrokeredMessage message) where T : class
        {
            if (message.Properties.TryGetValue("messageType", out object messageType))
            {
                var messageBodyType = Type.GetType(messageType.ToString());
                if (messageBodyType == null)
                {
                    //Should never get here as a messagebodytype should
                    //always be set BEFORE putting the message on the queue

                    message.DeadLetter();
                }


                //read body only if event handler hooked
                var method = typeof(BrokeredMessage).GetMethod("GetBody", new Type[] { });
                var generic = method.MakeGenericMethod(messageBodyType);
                //try
                //{
                var messageBody = generic.Invoke(message, null);
                return (T)messageBody;
                //DoSomethingWithYourData();
                //receivedMessage.Complete();
                // }
                //catch (Exception e)
                //{
                //    Debug.Write("Can not handle message. Abandoning.");
                //    receivedMessage.Abandon();
                //}
            }
            message.DeadLetter();
            return null;
        }
    }
}