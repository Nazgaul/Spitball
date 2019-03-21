using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserProfileAboutQuery : IQuery<IEnumerable<CourseDto>>
    {
        public UserProfileAboutQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; set; }



        internal sealed class UserProfileAboutQueryHandler : IQueryHandler<UserProfileAboutQuery, IEnumerable<CourseDto>>
        {
            private readonly DapperRepository _repository;

            public UserProfileAboutQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(UserProfileAboutQuery query, CancellationToken token)
            {
                using (var conn = _repository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(@"select CourseId as Name from sb.UsersCourses
where UserId = @id", new { id = query.UserId });
                }
            }
        }
    }
}