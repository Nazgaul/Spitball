using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class StudyRoomQuery : IQuery<StudyRoomDto>
    {
        public StudyRoomQuery(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private Guid Id { get; set; }

        private long UserId { get;  }

       


        internal sealed class StudyRoomQueryHandler : IQueryHandler<StudyRoomQuery, StudyRoomDto>
        {
            private readonly DapperRepository _repository;

            public StudyRoomQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<StudyRoomDto> GetAsync(StudyRoomQuery query, CancellationToken token)
            {
                
                using (var conn = _repository.OpenConnection())
                {
                    
                    var result = await conn.QueryFirstOrDefaultAsync<StudyRoomDto>(@"Select onlineDocumentUrl as OnlineDocument 
from sb.StudyRoom
cross apply STRING_SPLIT(identifier,'_')
where id = @Id
and value = @UserId", new { query.Id, query.UserId });

                    return result;
                }
                
            }
        }
    }
}