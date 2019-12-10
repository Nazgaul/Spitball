using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserSalesByIdQuery : IQuery<IEnumerable<SaleDto>>
    {
        public UserSalesByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }

        internal sealed class UserSalesByIdQueryHandler : IQueryHandler<UserSalesByIdQuery, IEnumerable<SaleDto>>
        { 
            private readonly IDapperRepository _dapper;

            public UserSalesByIdQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<SaleDto>> GetAsync(UserSalesByIdQuery query, CancellationToken token)
            {
                const string sql = @"select
                                        COALESCE(q.Text, d.Name) as Info,
                                        COALESCE(d.DocumentType, TransactionType) as [Type],
                                        'Paid' as [Status],
                                        t.Created as [Date],
                                        t.Price
                                    from sb.[Transaction] t
                                    left join sb.Question q
	                                    on t.QuestionId = q.Id
                                    left join sb.Document d
	                                    on t.DocumentId = d.Id
                                    where user_id = @UserId and TransactionType in ('Document','Question')
                                    and t.[Type] = 'Earned'
                                    union
                                    select 
                                        'Tutoring Sessuion' as info,
                                        'TutoringSession' as [Type],
                                        case when srs.Receipt is null then 'Pending'
	                                        else 'Paid' end as [Status],
                                        srs.Created as [Date],
                                        srs.Price
                                    from sb.StudyRoom sr
                                    join sb.StudyRoomSession srs
	                                    on sr.Id = srs.StudyRoomId
                                    where sr.TutorId = @UserId
                                    order by [Date] desc";

                using (var conn = _dapper.OpenConnection())
                {
                    return await conn.QueryAsync<SaleDto>(sql, new { userId = query.Id });
                }
            }
        }
    }
}
