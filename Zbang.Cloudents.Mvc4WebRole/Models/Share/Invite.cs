using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class Invite
    {
        [Required(ErrorMessageResourceType = typeof(InvitationResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(InvitationResources), Name = "To")]
        public string[] Recepients { get; set; }

        [Required]
        public long BoxUid { get; set; }
    }
}