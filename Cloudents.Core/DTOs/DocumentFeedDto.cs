using System;
using System.Runtime.Serialization;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class DocumentFeedDto
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string University { get; set; }
        [DataMember]
        public string Course { get; set; }
        [DataMember]
        public string Snippet { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Professor { get; set; }
        //[DataMember]
        public DocumentType? TypeStr { get; set; }

        [DataMember]
        public string Type => TypeStr?.ToString("G");
        [DataMember]
        public UserDto User { get; set; }
        [DataMember]
        public int? Views { get; set; }

        [DataMember]
        public int? Downloads { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public DateTime? DateTime { get; set; }

        [DataMember] public VoteDto Vote { get; set; }
    }

    public class VoteDto
    {
        public int Votes { get; set; }
        public VoteType? Vote { get; set; }
    }

    public abstract class UserVoteDto<T>
    {
        public T Id { get; set; }
        public VoteType Vote { get; set; }
    }

    public class UserVoteDocumentDto : UserVoteDto<long>
    {
     
    }

    public class UserVoteQuestionDto : UserVoteDto<long>
    {

    }

    public class UserVoteAnswerDto : UserVoteDto<Guid?>
    {
      
    }
}