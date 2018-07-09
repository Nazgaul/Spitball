using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Cloudents.Core.DTOs
{
    [UsedImplicitly]
    public class UniversitySearchWriteDto : SearchWriteIsDeleted
    {
        public string Name { get; [UsedImplicitly] set; }
        public string Image { get; [UsedImplicitly] set; }
        public double Longitude { get; [UsedImplicitly] set; }
        public double Latitude { get; [UsedImplicitly] set; }
        public string Extra { get; [UsedImplicitly] set; }
    }

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