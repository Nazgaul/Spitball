using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class PaymentRequest
    {
        [Required]
        [StringLength(255)]
        public string UserKey { get; set; }
        [Required]
        [StringLength(255)]
        public string TutorKey { get; set; }
        [Required]
        [Range(5,50000)]
        public decimal Amount { get; set; }
        [Required]
        public Guid StudyRoomSessionId { get; set; }
    }
}
