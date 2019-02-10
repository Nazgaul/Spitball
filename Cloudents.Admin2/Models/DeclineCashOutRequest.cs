using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
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