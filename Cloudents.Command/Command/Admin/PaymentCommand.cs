using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentCommand: ICommand
    {
        public PaymentCommand(string userKey, string tutorKey, decimal anount)
        {
            UserKey = userKey;
            TutorKey = tutorKey;
            Anount = anount;
        }
        public string UserKey { get; set; }
        public string TutorKey { get; set; }
        public decimal Anount { get; set; }
    }
}
