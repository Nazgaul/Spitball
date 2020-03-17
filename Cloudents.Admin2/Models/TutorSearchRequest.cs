using Cloudents.Core.Enum;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class TutorSearchRequest
    {
        public string Term { get; set; }
        public ItemState? State { get; set; }
        public string Country { get; set; }
    }
}
