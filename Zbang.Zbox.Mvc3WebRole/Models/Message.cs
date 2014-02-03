using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Zbox.Mvc3WebRole.Models.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class Message
    {
        [Required(ErrorMessageResourceType = typeof(InvitationResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(InvitationResources), Name = "To")]
        public string[] Recepients { get; set; }

        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        //[Required]
        [HiddenInput(DisplayValue = false)]
        public string BoxUid { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ItemUid { get; set; }


        public override string ToString()
        {
            return string.Format("Recepients {0} PersonalNote {1} BoxUid {2} ItemUid {3} ", string.Join(";", Recepients), Note, BoxUid, ItemUid);

        }
    }
}