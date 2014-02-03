using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class InviteToBox
    {
        [DataMember]
        public IList<string> EmailList { get; set; }
        [DataMember]
        public string PersonalMessage { get; set; }
        public override string ToString()
        {
            return string.Format("  EmailList {0} PersonalMessage {1}",
                   string.Join(";@", EmailList), PersonalMessage);
        }
    }
}