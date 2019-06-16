using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Admin2.Models
{
    public class ChangeConversationStatusRequest
    {
        public ChatRoomStatus Status { get; set; }
    }
}
