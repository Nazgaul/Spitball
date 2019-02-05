using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class ApproveCashOutRequest
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        [Required]
        public Guid TransactionId { get; set; }
    }


    public class DeclineCashOutRequest
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        [Required]
        public Guid TransactionId { get; set; }

        /// <summary>
        /// Decline Reason
        /// </summary>
        [Required]
        public string Reason { get; set; }


    }
}
