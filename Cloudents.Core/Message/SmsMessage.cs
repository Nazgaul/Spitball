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
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_message))
        //        {
        //            return null;
        //        }
        //        return $"Your code to enter into Spitball is : {_message}";
        //    }
        //    set => _message = value;
        //}

    }
}