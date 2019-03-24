using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class UpdateSettingsRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Description { get; set; }
    }

    public class BecomeTutorRequest
    {
        public string Bio { get; set; }
        public decimal Price { get; set; }
    }
}
