using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Query
{
    public class UserDataByIdQuery : IQuery<RegularUser>,
        IQuery<IEnumerable<BalanceDto>>,
        IQuery<IEnumerable<TransactionDto>> 
       
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

        public long UserId { get;}
    }
}