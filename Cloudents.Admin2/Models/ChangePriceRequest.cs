using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class ChangePriceRequest
    {
        [Range(1,long.MaxValue)]
        public long TutorId { get; set; }
        [Range(0,50000)]
        public decimal Price { get; set; }
    }
}