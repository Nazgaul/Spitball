using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class UnSuspendTutorRequest
    {
        [Range(1, long.MaxValue)]
        public long TutorId { get; set; }
    }
}
