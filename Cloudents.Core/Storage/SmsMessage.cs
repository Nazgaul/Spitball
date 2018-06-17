using System.Runtime.Serialization;

namespace Cloudents.Core.Storage
{
    [DataContract]
    public class SmsMessage 
    {
        private string _message;

        public SmsMessage(string phoneNumber, string code)
        {
            PhoneNumber = phoneNumber;
            Message = code;
        }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Message
        {
            get => $"Your code is: {_message}";
            set => _message = value;
        }
    }
}