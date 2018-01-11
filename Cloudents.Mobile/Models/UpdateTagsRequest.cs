using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class UpdateTagsRequest
    {
        [Required]
        public string[] Tags { get; set; }  
    }
}