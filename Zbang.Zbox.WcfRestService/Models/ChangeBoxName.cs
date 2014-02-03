using System.Runtime.Serialization;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class ChangeBoxName
    {
        [DataMember]
        public string NewBoxName { get; set; }

        public override string ToString()
        {
            return string.Format("  newBoxName {0}",
                    NewBoxName);

        }
    }
}