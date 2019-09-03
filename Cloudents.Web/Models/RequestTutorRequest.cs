using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RequestTutorRequest
    {
        public string  Name { get; set; }
        [EmailAddress]
        public string  Email { get; set; }
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$")]
        public string  Phone { get; set; }
        public string Text { get; set; }
        [Required]
        public string Course { get; set; }

        public long? TutorId { get; set; }

        public bool MoreTutors { get; set; }
    }
}