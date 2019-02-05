﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;

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
        public string Professor { get; set; }

        public string Type { get; set; }
        public UserDto User { get; set; }
        public int? Views { get; set; }

        public int? Downloads { get; set; }

        public string Url { get; set; }

        public string Source { get; set; }

        public DateTime? DateTime { get; set; }

         public VoteDto Vote { get; set; }

        public decimal? Price { get; set; }

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