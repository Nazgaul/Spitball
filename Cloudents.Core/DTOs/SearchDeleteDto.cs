using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class SearchWriteBaseDto
    {
        [DataMember]
        public long Id { get; set; }
        public long Version { get; set; }
    }

    [DataContract]
    public class SearchWriteIsDeleted : SearchWriteBaseDto
    {
        public bool IsDeleted { get; set; }
    }
}