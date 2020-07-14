using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class UnSuspendTutorRequest
    {
        [Range(1, long.MaxValue)]
        public long TutorId { get; set; }
    }


    public class TutorSubscriptionRequest
    {
        [Range(1, long.MaxValue)]
        public long TutorId { get; set; }

        [Range(1, double.MaxValue)]
        public double Price { get; set; }
    }
}
