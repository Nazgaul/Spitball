using Cloudents.Core.Enum;

namespace Cloudents.Admin2.Models
{
    public class CoursesRequest
    {
        public string Language { get; set; }
        public ItemState? State { get; set; }
        public string Filter { get; set; }
    }
}
