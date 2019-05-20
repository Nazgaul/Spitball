using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Entities;

namespace Cloudents.Web.Models
{
    public class UpdateSettingsRequest
    {
        [Required]
        [StringLength(255, ErrorMessage = "StringLength")]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Description { get; set; }

        [StringLength(1000, ErrorMessage = "StringLength")]
        public string Bio { get; set; }

        [Range(Tutor.MinimumPrice, Tutor.MaximumPrice)]
        public decimal Price { get; set; }
    }


   
}
