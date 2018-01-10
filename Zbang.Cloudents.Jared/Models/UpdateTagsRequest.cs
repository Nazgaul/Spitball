using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class UpdateTagsRequest
    {
        [Required]
        public string[] Tags { get; set; }  
    }
}