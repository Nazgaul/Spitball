using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.QueryHandler
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


    public class UserDataByIdQueryHandler : IQueryHandlerAsync<long, User>
    {
        private readonly IUserRepository _userRepository;

        public UserDataByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> GetAsync(long query, CancellationToken token)
        {
            return _userRepository.GetAsync(query, token);
        }
    }


    public class UserAccountDataQueryHandler : IQueryHandlerAsync<long, UserAccountDto>
    {
        private readonly IUserRepository _userRepository;

        public UserAccountDataQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<UserAccountDto> GetAsync(long query, CancellationToken token)
        {
            return _userRepository.GetUserDetailAsync(query, token);
        }
    }
}