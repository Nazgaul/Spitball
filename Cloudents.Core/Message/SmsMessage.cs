namespace Cloudents.Core.Message
{
    public class SmsMessage
    {
        public SmsMessage(string phoneNumber, string code)
        {
            PhoneNumber = phoneNumber;
            Message = code;
        }

        public string PhoneNumber { get; set; }

        public string Message { get; set; }
      

    }
}