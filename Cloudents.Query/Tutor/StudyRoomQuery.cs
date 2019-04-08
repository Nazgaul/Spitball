﻿using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Threading;
using System.Threading.Tasks;

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

        private long UserId { get; }




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
                    return await conn.QueryFirstOrDefaultAsync<StudyRoomDto>(@"
Select onlineDocumentUrl as OnlineDocument, cr.Id as ConversationId
from sb.StudyRoom sr join sb.ChatRoom cr on sr.Identifier = cr.Identifier
cross apply STRING_SPLIT(sr.identifier,'_')
where sr.id = @Id
and value = @UserId;", new { query.Id, query.UserId });



                }

            }
        }
    }
}