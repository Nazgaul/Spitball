using System;
using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class DocumentFeedWithFacetDto
    {
        public IEnumerable<DocumentFeedDto> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }

    public class DocumentFeedDto
    {
        public long Id { get; set; }
        public string University { get; set; }
        public string Course { get; set; }
        public string Snippet { get; set; }
        public string Title { get; set; }
        public DocumentUserDto User { get; set; }
        public int? Views { get; set; }

        public int? Downloads { get; set; }

        public string Url { get; set; }


        public DateTime? DateTime { get; set; }

         public VoteDto Vote { get; set; }

        public decimal? Price { get; set; }
        public string Preview { get; set; }
        public int Purchased { get; set; }

        public DocumentType DocumentType{ get; set; }
        public TimeSpan? Duration { get; set; }

    }

    public class DocumentUserDto
    {

        [EntityBind(nameof(ViewDocumentSearch.UserId))]
        public long Id { get; set; }
        [EntityBind(nameof(ViewDocumentSearch.UserName))]
        public string Name { get; set; }
        [EntityBind(nameof(ViewDocumentSearch.UserImage))]
        public string Image { get; set; }
        [EntityBind(nameof(ViewDocumentSearch.UserScore))]
        public int Score { get; set; }
        [EntityBind(nameof(ViewDocumentSearch.IsTutor))]
        public bool IsTutor { get; set; }
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

    public class UserVoteAnswerDto : UserVoteDto<Guid>
    {
        public UserVoteAnswerDto(Guid? id, VoteType vote)
        {
            Id = id ?? default;
            Vote = vote;
        }
    }
}