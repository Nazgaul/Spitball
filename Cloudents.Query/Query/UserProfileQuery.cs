using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Stuff;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

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
                BaseUser userAlias = null;
                ReadTutor readTutorAlias = null;
                University universityAlias = null;
                UserProfileDto userPoProfileDtoAlias = null;

                var googleExistsSubQuery = QueryOver.Of<GoogleTokens>().Where(w => w.Id == query.Id.ToString())
                    .ToRowCountQuery();
                    

                return await _session.QueryOver(() => userAlias)
                    .JoinAlias(x => x.University, () => universityAlias)
                    
                    .JoinEntityAlias(() => readTutorAlias, () => userAlias.Id == readTutorAlias.Id, JoinType.LeftOuterJoin)
                    .Where(w => w.Id == query.Id)
                    .And(() => userAlias.LockoutEnd < DateTime.UtcNow)
                    .SelectList(s2 =>
                    {
                        s2.Select(s => s.Id).WithAlias(() => userPoProfileDtoAlias.Id);
                        s2.Select(s => s.Image).WithAlias(() => userPoProfileDtoAlias.Image);
                        s2.Select(s => s.Name).WithAlias(() => userPoProfileDtoAlias.Name);
                        s2.Select(() => universityAlias.Name).WithAlias(() => userPoProfileDtoAlias.UniversityName);
                        s2.Select(s => s.Score).WithAlias(() => userPoProfileDtoAlias.Score);
                        s2.Select(() => userAlias.Description).WithAlias(() => userPoProfileDtoAlias.Description);
                        s2.Select(() => userAlias.Online).WithAlias(() => userPoProfileDtoAlias.Online);
                        s2.Select(Projections.Conditional(
                                Restrictions.Ge(Projections.SubQuery(googleExistsSubQuery), 0),
                                Projections.Constant(true), Projections.Constant(false)))
                            .WithAlias(() => userPoProfileDtoAlias.CalendarShared);
                        s2.Select(Projections.Property(() => readTutorAlias.Rate)
                            .As($"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.Rate)}"));
                        s2.Select(Projections.Property(() => readTutorAlias.RateCount).As(
                            $"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.ReviewCount)}"));
                        s2.Select(Projections.Property(() => userAlias.FirstName).As(
                            $"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.FirstName)}"));
                        s2.Select(Projections.Property(() => userAlias.LastName)
                            .As($"{nameof(userPoProfileDtoAlias.Tutor)}.{nameof(UserTutorProfileDto.LastName)}"));

                        s2.Select(Projections.Property(() => readTutorAlias.Price).As(
                            $"{nameof(userPoProfileDtoAlias.Tutor)}.{UserTutorProfileDto.TutorPriceVariableName}"));

                        s2.Select(Projections.Property(() => readTutorAlias.Country).As(
                            $"{nameof(userPoProfileDtoAlias.Tutor)}.{UserTutorProfileDto.TutorCountryVariableName}"));

                        return s2;
                    })
                    .TransformUsing(new DeepTransformer<UserProfileDto>())
                    .SingleOrDefaultAsync<UserProfileDto>(token);
            }
        }
    }
}