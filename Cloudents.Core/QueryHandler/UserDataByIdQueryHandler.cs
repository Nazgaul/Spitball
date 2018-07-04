using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;

namespace Cloudents.Core.QueryHandler
{
    public class UserDataByIdQueryHandler : IQueryHandler<UserDataByIdQuery, User>
    {
        private readonly IUserRepository _userRepository;

        public UserDataByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<User> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            return _userRepository.GetAsync(query.Id, token);
        }
    }
}