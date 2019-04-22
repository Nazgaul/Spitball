using System.Runtime.Serialization;

namespace Cloudents.Core.Message
{
    [DataContract]
    public class SmsMessage
    {
        public SmsMessage(string phoneNumber, string message, MessageType type)
        {
            PhoneNumber = phoneNumber;
            Message = message;
            Type = type;
        }


        protected SmsMessage()
        {
        }

        [DataMember]
        public string PhoneNumber { get; private set; }

        [DataMember]
        public string Message { get; private set; }

        public MessageType Type { get; }

        public sealed class MessageType
        {
            public MessageType(string type)
            {
                Type = type;
            }

            public string Type { get; }

            public static MessageType Sms = new MessageType("sms");
            public static MessageType Phone = new MessageType("phone");
        }
    }
}