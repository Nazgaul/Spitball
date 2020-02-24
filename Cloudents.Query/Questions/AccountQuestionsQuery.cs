using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Stuff;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Questions
{
    public class AccountQuestionsQuery : IQuery<IEnumerable<AccountQuestionDto>>
    {
        public AccountQuestionsQuery(long id, string country)
        {
            Id = id;
            Country = country;
        }
        public long Id { get; }
        public string Country { get; }

        internal sealed class AccountQuestionsQueryHandler : IQueryHandler<AccountQuestionsQuery, IEnumerable<AccountQuestionDto>>
        {
            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;
            public AccountQuestionsQueryHandler(QuerySession session, IUrlBuilder urlBuilder)
            {
                _session = session.StatelessSession;
                _urlBuilder = urlBuilder;
            }

            public async Task<IEnumerable<AccountQuestionDto>> GetAsync(AccountQuestionsQuery query, CancellationToken token)
            {
                const string sql = @"with cte as (
                                select top 1 * from (select 1 as o, u2.Id as UniversityId, COALESCE(u2.country,u.country) as Country, u.id as userid
                                 from sb.[user] u
                                 left join sb.University u2 on u.UniversityId2 = u2.Id
                                 where u.id = :userid
                                 union
                                 select 2,null,:country,0) t
                                 order by o
                                )

                                select top 50 q.Id,
				                                q.Text,
				                                q.Updated as [DateTime],
				                                u.Id as [User.Id],
				                                u.Name as [User.Name],
				                                u.ImageName as [User.Image]
                                from sb.Question q
                                join sb.[user] u
	                                on u.Id = q.UserId
                                ,cte
                                where not exists (select Id from sb.Answer where QuestionId = q.Id and State = 'Ok' and UserId = :userid) 
                                and q.Updated > GETUTCDATE() - 182
                                and q.State = 'Ok'
                                and q.userId != :userid
                                order by
                                case when q.CourseId in (select courseId from sb.usersCourses where userid = cte.userid) then 4 else 0 end +
                                case when q.UniversityId = cte.UniversityId then 3 else 0 end  +
                                cast(1 as float)/ISNULL(nullif( DATEDIFF(minute, q.Updated, GETUTCDATE()   ),0),1) desc";

                var res = await _session.CreateSQLQuery(sql)
                    .SetInt64("userid", query.Id)
                    .SetString("country", query.Country)
                    .SetResultTransformer(new DeepTransformer<AccountQuestionDto>('.', new SbAliasToBeanResultTransformer<AccountQuestionDto>()))
                    .ListAsync<AccountQuestionDto>(token);

                return res.Select(s =>
                {
                    s.User.Image = _urlBuilder.BuildUserImageEndpoint(s.User.Id, s.User.Image);
                    return s;
                });
            }
        }
    }
}
