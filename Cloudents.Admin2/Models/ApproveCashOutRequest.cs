using System;
using System.ComponentModel.DataAnnotations;

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
}
