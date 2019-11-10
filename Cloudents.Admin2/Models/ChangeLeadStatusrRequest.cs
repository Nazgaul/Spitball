using Cloudents.Core.Enum;
using System;
using System.ComponentModel.DataAnnotations;


namespace Cloudents.Admin2.Models
{
    public class ChangeLeadStatusrRequest
    {
        [Required]
        public ItemState State { get; set; }
        [Required]
        public Guid LeadId { get; set; }
    }
}
