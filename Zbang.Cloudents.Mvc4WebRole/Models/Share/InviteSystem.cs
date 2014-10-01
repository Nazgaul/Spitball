using System.Text;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Share
{
    public class InviteSystem
    {
        //[Required(ErrorMessageResourceType = typeof(InvitationResources), ErrorMessageResourceName = "FieldRequired")]
        //[Display(ResourceType = typeof(InvitationResources), Name = "To")]
        public string[] Recepients { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Recepients == null) return sb.ToString();
            foreach (var item in Recepients)
            {
                sb.AppendLine(" recipient : " + item);
            }
            return sb.ToString();
        }

    }
}