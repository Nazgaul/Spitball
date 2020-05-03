using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace Cloudents.Query.Admin
{
    public class StudyRoomQuery : IQueryAdmin2<IEnumerable<StudyRoomDto>>
    {
        public StudyRoomQuery(Country? country)
        {
            Country = country;
        }

        public Country? Country { get; }
        internal sealed class StudyRoomQueryHandler : IQueryHandler<StudyRoomQuery, IEnumerable<StudyRoomDto>>
        {

            private readonly IStatelessSession _statelessSession;


            public StudyRoomQueryHandler(QuerySession read)
            {
                _statelessSession = read.StatelessSession;
            }

            public async Task<IEnumerable<StudyRoomDto>> GetAsync(StudyRoomQuery query, CancellationToken token)
            {

                StudyRoomSession studyRoomSessionAlias = null!;
                StudyRoom studyRoomAlias = null!;
                StudyRoomUser studyRoomUserAlias = null!;
                User participantAlias = null!;
                Core.Entities.Tutor tutorAlias = null!;
                User userAlias = null!;
                StudyRoomDto resultAlias = null!;

                var queryOver = _statelessSession.QueryOver(() => studyRoomSessionAlias)
                    .JoinQueryOver(x => x.StudyRoom, () => studyRoomAlias)
                    .JoinQueryOver(() => studyRoomAlias.Users, () => studyRoomUserAlias)
                    .JoinQueryOver(() => studyRoomUserAlias.User, () => participantAlias)
                    // .JoinEntityAlias(() => tutorAlias, () => 638L == tutorAlias.Id)
                    .JoinQueryOver(() => studyRoomAlias.Tutor, () => tutorAlias)
                    .JoinQueryOver(() => tutorAlias.User, () => userAlias)
                    .Where(() =>
                        studyRoomSessionAlias.StudyRoomVersion == 0 || studyRoomSessionAlias.StudyRoomVersion == null)
                    .Where(() =>
                        studyRoomAlias.Id == studyRoomUserAlias.Room.Id &&
                        studyRoomAlias.Tutor.Id != studyRoomUserAlias.User.Id);

                if (query.Country != null)
                {
                    queryOver.Where(Restrictions.Eq(Projections.Property(() => userAlias.SbCountry), query.Country));
                    //queryOver.Where(() => userAlias.SbCountry == query.Country);
                }

                var future1 = queryOver.SelectList(list => list
                        .Select(s => s.Id).WithAlias(() => resultAlias.SessionId)
                        .Select(() => userAlias.Name).WithAlias(() => resultAlias.TutorName)
                        .Select(() => participantAlias.Name).WithAlias(() => resultAlias.UserName)
                        .Select(() => studyRoomSessionAlias.Created).WithAlias(() => resultAlias.Created)
                        .Select(() => studyRoomSessionAlias.Duration).WithAlias(() => resultAlias.DurationT) //duration
                        .Select(() => tutorAlias.Id).WithAlias(() => resultAlias.TutorId)
                        .Select(() => participantAlias.Id).WithAlias(() => resultAlias.UserId)

                    )
                    .TransformUsing(Transformers.AliasToBean<StudyRoomDto>()).Future<StudyRoomDto>();



                var newStudyRoomQuery = _statelessSession.QueryOver<StudyRoomSessionUser>()
                    .JoinAlias(x => x.StudyRoomSession, () => studyRoomSessionAlias)
                    .JoinAlias(x => x.User, () => participantAlias)
                    .JoinQueryOver(() => studyRoomSessionAlias.StudyRoom, () => studyRoomAlias)
                    .JoinQueryOver(() => studyRoomAlias.Tutor, () => tutorAlias)
                    .JoinQueryOver(() => tutorAlias.User, () => userAlias);
                if (query.Country != null)
                {
                    newStudyRoomQuery.Where(Restrictions.Eq(Projections.Property(() => userAlias.SbCountry),
                        query.Country));
                    //queryOver.Where(() => userAlias.SbCountry == query.Country);
                }
                var future2 = newStudyRoomQuery.SelectList(list => list
                        .Select(() => studyRoomSessionAlias.Id).WithAlias(() => resultAlias.SessionId)
                        .Select(() => userAlias.Name).WithAlias(() => resultAlias.TutorName)
                        .Select(() => participantAlias.Name).WithAlias(() => resultAlias.UserName)

                        .Select(() => studyRoomSessionAlias.Created).WithAlias(() => resultAlias.Created)
                        .Select(x => x.Duration).WithAlias(() => resultAlias.DurationT) //duration
                        .Select(() => tutorAlias.Id).WithAlias(() => resultAlias.TutorId)
                        .Select(() => participantAlias.Id).WithAlias(() => resultAlias.UserId)

                     )
                     .TransformUsing(Transformers.AliasToBean<StudyRoomDto>()).Future<StudyRoomDto>();


                var result = (await future1.GetEnumerableAsync(token)).Union(future2.GetEnumerable());

                return result;

            }
        }
    }
}
