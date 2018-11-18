using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Command
{
    public class FinishRegistrationCommand : ICommand
    {
        public FinishRegistrationCommand(long id)
        {
            Id = id;
        }
        public long Id { get; set; }
    }
}
