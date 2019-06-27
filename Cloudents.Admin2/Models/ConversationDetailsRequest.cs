using Cloudents.Core.Enum;

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
