using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class AnonymousRequestTutorRequest
    {
        public string Text { get; set; }
        public string Course { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string University { get; set; }
        public string PhoneNumber { get; set; }
        [MaxLength(4, ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }

    }
}
