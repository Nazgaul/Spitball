﻿namespace Cloudents.Core.Message
{
    public class SmsMessage2
    {
        public SmsMessage2(string phoneNumber, string code)
        {
            PhoneNumber = phoneNumber;
            Message = code;
        }

        public string PhoneNumber { get; set; }

        public string Message { get; set; }
    }
}