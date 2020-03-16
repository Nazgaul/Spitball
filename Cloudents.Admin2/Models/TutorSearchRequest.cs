using Cloudents.Core.Enum;

namespace Cloudents.Admin2.Models
{
    public class TutorSearchRequest
    {
        public string Term { get; set; }
        public ItemState? State { get; set; }
    }
}
