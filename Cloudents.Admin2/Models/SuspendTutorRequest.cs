using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class SuspendTutorRequest
    {
        [Range(1, long.MaxValue)]
        public long TutorId { get; set; }
    }
}
