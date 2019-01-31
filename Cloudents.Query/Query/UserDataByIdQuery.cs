﻿using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Models;

namespace Cloudents.Query.Query
{
    public class UserDataByIdQuery : IQuery<RegularUser>,
        IQuery<UserAccountDto>,
        IQuery<IEnumerable<BalanceDto>>,
        IQuery<IEnumerable<TransactionDto>>, 
        IQuery<UserProfileDto>
    {
        public UserDataByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class UserVotesByCategoryQuery : 
        IQuery<IEnumerable<UserVoteDocumentDto>>,
        IQuery<IEnumerable<UserVoteQuestionDto>>

    {
        public UserVotesByCategoryQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; set; }
    }

    public class UserVotesQuestionQuery : IQuery<IEnumerable<UserVoteAnswerDto>>
    {
        public UserVotesQuestionQuery(long userId, long questionId)
        {
            UserId = userId;
            QuestionId = questionId;
        }

        public long UserId { get; private set; }
        public long QuestionId { get; private set; }
    }


    public class UserWithUniversityQuery : IQuery<UserProfile>
    {
        public UserWithUniversityQuery(long id/*, Guid? universityId*/)
        {
            Id = id;
          //  UniversityId = universityId;
        }

        public long Id { get; }
        //public Guid? UniversityId { get; }
    }
}