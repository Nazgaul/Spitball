using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Zbang.Zbox.WebApi.Models
{
    public class InviteToBox
    {
        [Required]
        public IList<string> EmailList { get; set; }
        
        public string PersonalMessage { get; set; }
        public override string ToString()
        {
            return string.Format("  EmailList {0} PersonalMessage {1}",
                   string.Join(";@", EmailList), PersonalMessage);
        }
    }
}