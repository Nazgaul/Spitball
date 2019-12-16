using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateUniversityRequest
    {
        [StringLength(100, MinimumLength = 10, ErrorMessage = "StringLength")]

        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        public string Country { get; set; }
    }


    public class AssignUniversityRequest
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
