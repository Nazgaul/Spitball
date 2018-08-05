using System;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class ContactUsEmail : BaseEmail
    {
        public ContactUsEmail(string name, string email, string text) 
            : base("support@spitball.co", null, $"Contact us Ico {email}")
        {
            Name = name;
            Email = email;
            Text = text;
        }

        public ContactUsEmail(string email)
            : base("support@spitball.co", null, $"Subscribe Ico {email}")
        {
            Email = email;
        }

        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Text { get; private set; }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Text))
            {
                return $"Subscribe this email: {Email}";
            }
            return $"Contact us name: {Name}, email: {Email}, Text: {Text} ";

        }
    }
}