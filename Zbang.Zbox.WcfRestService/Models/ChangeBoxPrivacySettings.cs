using System.Runtime.Serialization;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class ChangeBoxPrivacySettings
    {
        [DataMember]
        public BoxPrivacySettings NewBoxPrivacySettings { get; set; }

        public override string ToString()
        {
            return string.Format(" NewBoxPrivacySettings {0}",
                    NewBoxPrivacySettings);

        }
    }
}