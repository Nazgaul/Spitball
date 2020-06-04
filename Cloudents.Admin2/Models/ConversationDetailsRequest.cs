using Cloudents.Core.Enum;

namespace Cloudents.Admin2.Models
{
    public class ConversationDetailsRequest
    {
        public long? Id { get; set; }
        public int Page { get; set; }
        public int? Status { get; set; }
        public string AssignTo { get; set; }
        public WaitingFor AutoStatus { get; set; }
    }
}
