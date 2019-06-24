using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class ConversationDetailsRequest
    {
        public long? Id { get; set; }
        public int Page { get; set; }
        public ChatRoomStatus? Status { get; set; }
        public ChatRoomAssign? AssignTo { get; set; }
    }
}
