using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail.EmailParameters
{
    public class StoreContactUs : MailParameters
    {
        public StoreContactUs(string name, string phone, string university, string email, string text)
            :base(new CultureInfo("he-IL"))
        {
            Name = name;
            Phone = phone;
            University = university;
            Email = email;
            Text = text;
        }
        public string Name { get; private set; }

        

        public string Phone { get; private set; }
        public string University { get; private set; }
        public string Email { get; private set; }
        public string Text { get; private set; }
        public override string MailResover
        {
            get { return StoreContactResolver; }
        }
    }
}
