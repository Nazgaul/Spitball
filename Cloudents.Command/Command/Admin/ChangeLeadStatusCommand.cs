using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command.Admin
{
    public class ChangeLeadStatusCommand : ICommand
    {
        public Guid LeadId { get; set; }
        public ItemState State { get; }
        public ChangeLeadStatusCommand(Guid leadId, ItemState state)
        {
            LeadId = leadId;
            State = state;
        }
    }
}
