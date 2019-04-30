using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Email
{
    public class PaymentDetailEmailQuery : IQuery<IEnumerable<EmailPaymentDto>>
    {
        public PaymentDetailEmailQuery(Guid studyRoomId)
        {
            StudyRoomId = studyRoomId;
        }

        public Guid StudyRoomId { get; private set; }
        

        internal sealed class PaymentDetailEmailQueryHandler : IQueryHandler<PaymentDetailEmailQuery, IEnumerable<EmailPaymentDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public PaymentDetailEmailQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<EmailPaymentDto>> GetAsync(PaymentDetailEmailQuery query, CancellationToken token)
            {
                const string sql = @"select u.id,u.Email,u.language, uTutor.Name as TutorName
 from sb.StudyRoomUser sru join sb.studyRoom sr on sru.StudyRoomId = sr.Id
join sb.[user] u on sru.UserId = u.Id 
join sb.[user] uTutor on sr.TutorId = uTutor.Id
where sr.Id = @id
and sr.TutorId <> sru.UserId
and (u.PaymentKey is null or u.PaymentKeyExpiration  < GETUTCDATE());";

                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<EmailPaymentDto>(sql, new {id = query.StudyRoomId});
                }
            }
        }
    }
}