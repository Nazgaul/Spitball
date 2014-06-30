using System;
using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class AddLink
    {
        [Required(ErrorMessageResourceType = typeof(AddLinkResources), ErrorMessageResourceName = "LinkRequired")]
        [RegularExpression(Zbox.Domain.Common.Validation.UrlRegex2, ErrorMessageResourceType = typeof(AddLinkResources), ErrorMessageResourceName = "NotValidUrl")]//.U"[-a-zA-Z0-9@:%_\\+~#?&//=]{2,256}\\.[a-z]{2,256}\\b(\\//?[\\-a-zA-Z0-9@:%_\\+\\.~#?&//,=!;()]*)?")]
        public string FileUrl { get; set; }
        [Required]
        public long BoxId { get; set; }

        public Guid? TabId { get; set; }
    }
}