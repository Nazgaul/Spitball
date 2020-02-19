using Cloudents.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UpdateSettingsRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string FirstName { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string LastName { get; set; }
        [StringLength(255, MinimumLength = 15, ErrorMessage = "StringLength")]
        public string Description { get; set; }

        [StringLength(1000, MinimumLength = 15, ErrorMessage = "StringLength")]
        public string Bio { get; set; }

        [Range(Tutor.MinimumPrice, Tutor.MaximumPrice)]
        public int? Price { get; set; }
    }



}
