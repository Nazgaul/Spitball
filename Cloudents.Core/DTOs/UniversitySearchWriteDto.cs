using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class UniversitySearchWriteDto : SearchWriteIsDeleted
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Image { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember] public string Extra { get; set; }
    }

    [DataContract]
    public class CourseSearchWriteDto : SearchWriteIsDeleted
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public long UniversityId { get; set; }
    }
}