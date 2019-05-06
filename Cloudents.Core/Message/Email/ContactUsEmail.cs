//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using JetBrains.Annotations;

//namespace Cloudents.Core.Message.Email
//{
//    /// <summary>
//    /// This is the email template from the ico site
//    /// </summary>
//    [Serializable]
//    public class ContactUsEmail : BaseEmail
//    {
//        public ContactUsEmail(string name, string email, string text)
//            : base("support@spitball.co", $"Contact us Ico {email}", null)
//        {
//            Name = name;
//            Email = email;
//            Text = text;
//        }

//        public ContactUsEmail(string email)
//            : base("support@spitball.co", $"Subscribe Ico {email}", null)
//        {
//            Email = email;
//        }

//        public string Name { get; private set; }
//        public string Email { get; private set; }
//        public string Text { get; private set; }
//        public override string ToString()
//        {
//            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Text))
//            {
//                return $"Subscribe this email: {Email}";
//            }
//            return $"Contact us name: {Name}, email: {Email}, Text: {Text} ";

//        }

//        public override string Campaign => "ContactUs";
//        protected override IDictionary<CultureInfo, string> Templates => null;
//    }
//}