using System.Runtime.Serialization;
using Cloudents.Core.Attributes;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    [BinaryIncludeSerialize(typeof(SearchWriteIsDeleted), 1)]
    public class SearchWriteBaseDto
    {
        [DataMember(Order = 2)]
        public long Id { get; set; }

        public long Version { get; set; }
    }

    [DataContract]
    [BinaryIncludeSerialize(typeof(CourseSearchWriteDto), 1)]

    public class SearchWriteIsDeleted : SearchWriteBaseDto
    {
        public bool IsDeleted { get; set; }
    }
}