using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class AddLinkRequest
    {
        [Required]
        [RegularExpression(Zbox.Domain.Common.Validation.UrlRegex2)]
        public string FileUrl { get; set; }
        [Required]
        public long BoxId { get; set; }

       

        public bool Question { get; set; }

        public string Name { get; set; }
    }
}