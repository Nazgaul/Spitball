using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

namespace Cloudents.Core.QueryHandler
{
    public class UserDataQueryHandler : IQueryHandler<UserDataExpressionQuery, User>
    {
        private readonly IUserRepository _userRepository;

        public UserDataQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public  Task<User> GetAsync(UserDataExpressionQuery query, CancellationToken token)
        {
           return _userRepository.GetUserByExpressionAsync(query.QueryExpression, token);
        }
    }
}