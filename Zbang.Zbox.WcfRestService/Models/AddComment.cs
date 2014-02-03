using System.Runtime.Serialization;

namespace Zbang.Zbox.WcfRestService.Models
{
    [DataContract]
    public class AddComment
    {
        [DataMember]
        public string Comment { get; set; }

        public override string ToString()
        {
            return string.Format("  comment {0}",
                    Comment);

        }
    }
}