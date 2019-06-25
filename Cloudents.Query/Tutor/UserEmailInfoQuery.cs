using System.Linq;
using Cloudents.Core.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class UserEmailInfoQuery: IQuery<UserEmailInfoDto>
    {
        public UserEmailInfoQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get;  }

        internal sealed class UserEmailInfoQueryHandler : IQueryHandler<UserEmailInfoQuery, UserEmailInfoDto>
        {
            private readonly IStatelessSession _statelessSession;
            public UserEmailInfoQueryHandler(QuerySession querySession)
            {
                _statelessSession = querySession.StatelessSession;
            }
            public async Task<UserEmailInfoDto> GetAsync(UserEmailInfoQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<User>()
                    .Where(w => w.Id == query.UserId)
                    .Select(s => new UserEmailInfoDto()
                    {
                        Name = s.Name, Email = s.Email, University = s.University.Id, PhoneNumber = s.PhoneNumber
                    }).SingleOrDefaultAsync(token);


                //using (var conn = _dapperRepository.OpenConnection())
                //{
                //    const string sql = @"select Email, u.Name, un.Name as University, u.Country, 
                //                            u.PhoneNumberHash as PhoneNumber
                //                        from sb.[User] u
                //                        left join sb.University un
                //                         on u.UniversityId2 = un.Id
                //                        where u.Id = @UserId";
                //    return await conn.QueryFirstOrDefaultAsync<UserEmailInfoDto>(sql, new { query.UserId });
                //}
            }
        }
    }
}
