using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Query.Stuff;
using NHibernate;

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
t.price as Tutor_TutorPrice, 
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

                //BaseUser userAlias = null;
                //ReadTutor readTutorAlias = null;
                //University universityAlias = null;
                //UserProfileDto userPoProfileDtoAlias = null;

                //var googleExistsSubQuery = QueryOver.Of<GoogleTokens>().Where(w => w.Id == query.Id.ToString())
                //    .ToRowCountQuery();
                    

                //return await _session.QueryOver(() => userAlias)
                //    .JoinAlias(x => x.University, () => universityAlias)
                    
                //    .JoinEntityAlias(() => readTutorAlias, () => userAlias.Id == readTutorAlias.Id, JoinType.LeftOuterJoin)
                //    .Where(w => w.Id == query.Id)
                //    //.And(() => userAlias.LockoutEnd < DateTime.UtcNow)
                //    .SelectList(s2 =>
                //    {
                //        s2.Select(s => s.Id).WithAlias(() => userPoProfileDtoAlias.Id);
                //        s2.Select(s => s.Image).WithAlias(() => userPoProfileDtoAlias.Image);
                //        s2.Select(s => s.Name).WithAlias(() => userPoProfileDtoAlias.Name);
                //        s2.Select(() => universityAlias.Name).WithAlias(() => userPoProfileDtoAlias.UniversityName);
                //        s2.Select(s => s.Score).WithAlias(() => userPoProfileDtoAlias.Score);
                //        //s2.Select(() => userAlias.Description).WithAlias(() => userPoProfileDtoAlias.Description);
                //        //s2.Select(() => userAlias.Online).WithAlias(() => userPoProfileDtoAlias.Online);
                //        s2.Select(Projections.Conditional(
                //                Restrictions.Ge(Projections.SubQuery(googleExistsSubQuery), 0),
                //                Projections.Constant(true), Projections.Constant(false)))
                //            .WithAlias(() => userPoProfileDtoAlias.CalendarShared);
                //        s2.Select(Projections.Property(() => readTutorAlias.Rate)
                //            .As($"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.Rate)}"));
                //        s2.Select(Projections.Property(() => readTutorAlias.RateCount).As(
                //            $"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.ReviewCount)}"));
                //        //s2.Select(Projections.Property(() => userAlias.FirstName).As(
                //        //    $"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.FirstName)}"));
                //        //s2.Select(Projections.Property(() => userAlias.LastName)
                //        //    .As($"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.LastName)}"));

                //        s2.Select(Projections.Property(() => readTutorAlias.Price).As(
                //            $"{nameof(userPoProfileDtoAlias.Tutor)}.{UserTutorProfileDto.TutorPriceVariableName}"));

                //        s2.Select(Projections.Property(() => readTutorAlias.Country).As(
                //            $"{nameof(userPoProfileDtoAlias.Tutor)}.{UserTutorProfileDto.TutorCountryVariableName}"));

                //        return s2;
                //    })
                //    .TransformUsing(new DeepTransformer<UserProfileDto>())
                //    .SingleOrDefaultAsync<UserProfileDto>(token);
            }
        }
    }
}