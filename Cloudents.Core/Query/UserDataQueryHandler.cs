using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class UserDataQueryHandler : IQueryHandlerAsync<Expression<Func<User, bool>>, User>
    {
        private readonly IUserRepository _userRepository;

        public UserDataQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public  Task<User> GetAsync(Expression<Func<User, bool>> query, CancellationToken token)
        {
           return _userRepository.GetUserByExpressionAsync(query, token);
        }
    }
}