using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using System.Collections.Generic;

namespace Cloudents.Query.Users
{
    public class UserDataByIdQuery : IQuery<User>,
        //IQuery<IEnumerable<BalanceDto>>,
        IQuery<IEnumerable<TransactionDto>>

    {
        public UserDataByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }



    public class UserVotesByCategoryQuery :
        IQuery<IEnumerable<UserVoteDocumentDto>>


    {
        public UserVotesByCategoryQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }
    }
}