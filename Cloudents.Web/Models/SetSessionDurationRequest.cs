using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetSessionDurationRequest
    {
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public TimeSpan RealDuration { get; set; }
    }
}
