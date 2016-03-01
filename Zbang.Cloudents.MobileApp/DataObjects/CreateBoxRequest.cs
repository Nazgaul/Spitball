using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Domain;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class CreateBoxRequest
    {
        [Required]
        [StringLength(Box.NameLength)]
        public string BoxName { get; set; }
    }
}