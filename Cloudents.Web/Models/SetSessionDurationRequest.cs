using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetSessionDurationRequest
    {
        [Required]
        public Guid SessionId { get; set; }

        public long? UserId { get; set; }

        [Required]
        [Range(1, 1000)]
        public long DurationInMinutes { get; set; }

    }
}
