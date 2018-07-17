using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class UserDataByIdQuery : IQuery<User>,
        IQuery<UserAccountDto>,
        IQuery<IEnumerable<BalanceDto>>,
        IQuery<IEnumerable<TransactionDto>>, 
        IQuery<ProfileDto>

    {
        public UserDataByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}