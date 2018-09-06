using System.Runtime.Serialization;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities.Search;

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


    public class AzureSyncBaseDto<T>
    {
        public long Id { get; set; }

        public string SYS_CHANGE_OPERATION { get; set; }

        public long SYS_CHANGE_VERSION { get; set; }

        public T Data { get; set; }
    }

    //public class QuestionAzureSyncDto : AzureSyncBaseDto
    //{
    //    public Question Question { get; set; }
    //}
}