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
                const string sql = @"select COALESCE(q.Id, d.Id) as Id,
                                    COALESCE(q.CourseId, d.CourseName) as Course,
                                        COALESCE(q.Text, d.Name) as Info,
                                        COALESCE(d.DocumentType, TransactionType) as [Type],
                                        'Paid' as [Status],
                                        t.Created as [Date],
                                        t.Price,
	                                    '' as StudentName,
	                                    0 as Duration,
	                                    case when q.Id is not null then (select a.Text from sb.Answer a where a.QuestionId = q.Id and a.UserId = @UserId) end as AnswerText
                                    from sb.[Transaction] t
                                    left join sb.Question q
	                                    on t.QuestionId = q.Id
                                    left join sb.Document d
	                                    on t.DocumentId = d.Id
                                    where user_id = @UserId and TransactionType in ('Document','Question')
                                    and t.[Type] = 'Earned'
                                    union

                                    select 0 as Id,
                                    '' as course,
                                        'Tutoring Sessuion with' as info,
                                        'TutoringSession' as [Type],
                                        case when srs.Receipt is null then 'Pending'
	                                        else 'Paid' end as [Status],
                                        srs.Created as [Date],
                                        srs.Price,
	                                    (
		                                    select u.Name 
		                                    from sb.[user] u 
		                                    join sb.StudyRoomUser sru 
			                                    on u.Id = sru.UserId 
		                                    where sru.StudyRoomId = sr.Id and sru.UserId != sr.TutorId
	                                    ) as StudentName,
	                                    srs.DurationInMinutes as Duration,
	                                    '' as AnswerText
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
