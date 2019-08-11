using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class PaymentRequest
    {
        //[Required]
        //[StringLength(255)]
        //public string UserKey { get; set; }
        [Range(5, 50000)]
        public decimal StudentPay { get; set; }
        [Range(5, 50000)]
        public decimal SpitballPay { get; set; }

        public long UserId { get; set; }
        public long TutorId { get; set; }


        //[Required]
        //[StringLength(255)]
        //public string TutorKey { get; set; }
        //[Required]
        //[Range(5,50000)]
        //public decimal Amount { get; set; }
        [Required]
        public Guid StudyRoomSessionId { get; set; }
    }
}
