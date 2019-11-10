using Cloudents.Core.DTOs;
using Cloudents.Query.Stuff;
using NHibernate;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserProfileQuery : IQuery<UserProfileDto>
    {
        public UserProfileQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }


        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
        internal sealed class UserProfileQueryHandler : IQueryHandler<UserProfileQuery, UserProfileDto>
        {

            private readonly IStatelessSession _session;

            public UserProfileQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<UserProfileDto> GetAsync(UserProfileQuery query, CancellationToken token)
            {

                const string sql = @"select u.id,
u.Image,
u.Name,
u2.name as universityName,
u.Score,
u.description,
u.online,
cast ((select count(*) from sb.GoogleTokens gt where u.Id = gt.Id) as bit) as CalendarShared,
t.rate as Tutor_Rate,
t.rateCount as Tutor_ReviewCount,
u.FirstName as Tutor_FirstName ,u.LastName as Tutor_LastName,
t.price as Tutor_Price, 
t.country as Tutor_TutorCountry

from sb.[user] u 
left join sb.[University] u2 on u.UniversityId2 = u2.Id
left join sb.readTutor t 
	on U.Id = t.Id 

where u.id = :Id
and (u.LockoutEnd is null or u.LockoutEnd < GetUtcDate())";

                var sqlQuery = _session.CreateSQLQuery(sql);
                sqlQuery.SetInt64("Id", query.Id);
                sqlQuery.SetResultTransformer(new DeepTransformer<UserProfileDto>('_'));
                var result = await sqlQuery.UniqueResultAsync<UserProfileDto>(token);

                return result;
            }
        }
    }
}