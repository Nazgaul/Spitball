using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        public string  Name { get; set; }
        public string  Email { get; set; }
        public string  Phone { get; set; }
        public string Text { get; set; }
        [Required]
        public string Course { get; set; }

        public Guid? University { get; set; }
    }
}