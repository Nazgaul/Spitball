using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class Message
    {
        public Message()
        {
        }
        [Required(ErrorMessageResourceType = typeof(InvitationResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(InvitationResources), Name = "To")]
        public string[] Recepients { get; set; }

        [DataType(DataType.MultilineText)]
        [Required]
        public string Note { get; set; }

        public override string ToString()
        {
            return string.Format("Recepients {0} PersonalNote {1}  ", 
                string.Join(";", Recepients), Note);

        }
    }
}