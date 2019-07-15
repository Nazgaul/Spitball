using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailDocumentsQuery : IQuery<IEnumerable<DocumentEmailDto>>
    {
        public GetUpdatesEmailDocumentsQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class GetUpdatesEmailDocumentsQueryHandler : IQueryHandler<GetUpdatesEmailDocumentsQuery, IEnumerable<DocumentEmailDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public GetUpdatesEmailDocumentsQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<DocumentEmailDto>> GetAsync(GetUpdatesEmailDocumentsQuery query, CancellationToken token)
            {
                const string sql = @"select d.[Name] as [FileName], d.Id as FileId,
	                                    u.Name as Uploader
                                    from sb.Document d
                                    join sb.[User] u
	                                    on u.Id = d.UserId 
                                    where  d.CreationTime > getutcdate() - 10 and d.CourseName in (select courseId 
                                                                                                from sb.UsersCourses 
                                                                                                where UserId = @UserId
                                                                                                    )
                                    and u.Id != @UserId and d.state = 'Ok'";
                using (var connection = _dapperRepository.OpenConnection())
                {
                    return await connection.QueryAsync<DocumentEmailDto>(sql, new { query.UserId });
                }
            }
        }
    }
}
