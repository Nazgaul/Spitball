using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Mvc3WebRole.Models.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class AddLink
    {
        [Required(ErrorMessageResourceType = typeof(AddLinkResources), ErrorMessageResourceName = "LinkRequired")]
        [RegularExpression(Zbang.Zbox.Domain.Common.Validation.UrlRegex2, ErrorMessageResourceType = typeof(AddLinkResources), ErrorMessageResourceName = "NotValidUrl")]//.U"[-a-zA-Z0-9@:%_\\+~#?&//=]{2,256}\\.[a-z]{2,256}\\b(\\//?[\\-a-zA-Z0-9@:%_\\+\\.~#?&//,=!;()]*)?")]
        [DataType(DataType.Url)]
        public string Url { get; set; }
        [Required]
        public string BoxId { get; set; }
    }
}