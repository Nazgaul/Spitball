using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class InviteSystem
    {
        //[Required(ErrorMessageResourceType = typeof(InvitationResources), ErrorMessageResourceName = "FieldRequired")]
        //[Display(ResourceType = typeof(InvitationResources), Name = "To")]
        public string[] Recepients { get; set; }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Recepients)
            {
                sb.AppendLine(" recepient : " + item);
            }
            return sb.ToString();
        }

    }
}