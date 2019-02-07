using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class ConfirmePhoneNumberCommand : ICommand
    {
        public ConfirmePhoneNumberCommand(long id)
        {
            Id = id;
        }
        public long Id { get; set; }
    }
}
