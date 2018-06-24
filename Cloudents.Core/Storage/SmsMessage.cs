using System.Runtime.Serialization;

namespace Cloudents.Core.Storage
{
    [DataContract]
    public class SmsMessage 
    {
        [DataMember]
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
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    return null;
                }
                return $"Your code to enter into Spitball is : {_message}";
            }
            set => _message = value;
        }
    }
}