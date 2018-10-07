using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{

    [DataContract]
    public class CourseSearchWriteDto : SearchWriteIsDeleted
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Code { get; set; }

        [DataMember(Order = 3)]
        public long UniversityId { get; set; }
    }
}