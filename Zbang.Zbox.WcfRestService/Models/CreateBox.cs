using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class CreateBox
    {
        [DataMember]        
        public string BoxName { get; set; }
        [DataMember]
        public NotificationSettings NotificationSettings { get; set; }
        [DataMember]
        public BoxPrivacySettings BoxPrivacySettings { get; set; }

        public override string ToString()
        {
            return string.Format("  boxName {0} NotificationSettings {1} BoxPrivacySettings {2}",
                     BoxName, NotificationSettings, BoxPrivacySettings);

        }

    }
}