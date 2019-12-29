using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class UserStudyRoomQuery : IQuery<IEnumerable<UserStudyRoomDto>>
    {
        public UserStudyRoomQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        internal sealed class UserStudyRoomQueryHandler : IQueryHandler<UserStudyRoomQuery, IEnumerable<UserStudyRoomDto>>
        {
            private readonly IStatelessSession _session;
            //private readonly IDapperRepository _dapperRepository;

            public UserStudyRoomQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserStudyRoomDto>> GetAsync(UserStudyRoomQuery query, CancellationToken token)
            {
                StudyRoom studyRoomAlias = null;
                //StudyRoomSession studyRoomSessionAlias = null;
                StudyRoomUser studyRoomUserAlias = null;
                User userAlias = null;

                UserStudyRoomDto resultAlias = null;


                //_session.Query<StudyRoomUser>()
                //    .Fetch(f=>f.Room)
                //    .ThenFetch(f=>f.Users)
                var detachedQuery = QueryOver.Of<StudyRoomUser>()
                    .Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Room.Id);

                return await _session.QueryOver(() => studyRoomAlias)
                    .JoinAlias(x => x.Users, () => studyRoomUserAlias)
                    .JoinEntityAlias(() => userAlias,
                        () => userAlias.Id == studyRoomUserAlias.User.Id && userAlias.Id != query.UserId)
                    .WithSubquery.WhereProperty(x => x.Id).In(detachedQuery)
                    .SelectList(sl =>
                            sl.Select(s => userAlias.Name).WithAlias(() => resultAlias.Name)
                                .Select(s => userAlias.ImageName).WithAlias(() => resultAlias.Image)
                                .Select(s => userAlias.Online).WithAlias(() => resultAlias.Online)
                                .Select(s => userAlias.Id).WithAlias(() => resultAlias.UserId)
                                .Select(s => s.Id).WithAlias(() => resultAlias.Id)
                                .Select(s => s.DateTime.CreationTime).WithAlias(() => resultAlias.DateTime)
                                .Select(s => s.Identifier).WithAlias(() => resultAlias.ConversationId)
                                .Select(Projections.SqlFunction("coalesce", NHibernateUtil.DateTime,
                                                    Projections.Property(() => studyRoomAlias.DateTime.UpdateTime),
                                                    Projections.Property(() => studyRoomAlias.DateTime.CreationTime)))
                                    .WithAlias(() => resultAlias.LastSession)
                    ) 
                    .OrderBy(Projections.SqlFunction("COALESCE",NHibernateUtil.Object,
                        Projections.Property(() => studyRoomAlias.DateTime.UpdateTime),
                        Projections.Property(() => studyRoomAlias.DateTime.CreationTime))).Desc
                    //TODO on nhibernate 5.3 need to fix.
                    .TransformUsing(Transformers.AliasToBean<UserStudyRoomDto>())
                    .ListAsync<UserStudyRoomDto>(token);/*.OrderByDescending(o => o.LastActive > o.DateTime ? o.LastActive : o.DateTime)*/;

                //var t = await _session.Query<StudyRoom>()
                //    .FetchMany(f => f.Sessions)
                //    .FetchMany(f => f.Users)
                //    .ThenFetch(f => f.User)
                //    .Where(w => _session.Query<StudyRoomUser>().Where(w => w.User.Id == query.UserId).Select(s => s.Room.Id).Contains(w.Id))
                //   .Where(w => w.Users.All(a => a.User.Id != query.UserId)).ToListAsync(token);
                //    //.Select(s => new UserStudyRoomDto()
                //    //{
                //    //    Name = s.Users.Select(s => s.User.Name).FirstOrDefault(),
                //    //    //Image = s.Users.Where(w => w.User.Id != query.UserId).Select(s => s.User.Image).FirstOrDefault(),
                //    //    //Online = s.Users.Where(w => w.User.Id != query.UserId).Select(s => s.User.Online).FirstOrDefault(),
                //    //    //UserId = s.Users.Where(w => w.User.Id != query.UserId).Select(s => s.User.Id).FirstOrDefault(),
                //    //    Id = s.Id,
                //    //    DateTime = s.DateTime,
                //    //    ConversationId = s.Identifier,
                //    //    LastActive = s.Sessions.Select(s => s.Created).OrderByDescending(o => o).FirstOrDefault()
                //    //}).OrderByDescending(o => o.LastActive).ToListAsync(token);
                //return null;

                //                using (var connection = _dapperRepository.OpenConnection())
                //                {
                //                    var result = await connection.QueryAsync<UserStudyRoomDto>(@"Select
                // u.Name as Name,
                // u.Image as Image,
                // u.Online as online,
                // u.Id as userId,
                // sr.Id as id,
                // sr.DateTime,
                // sr.Identifier as conversationId
                //from sb.StudyRoom sr
                //join sb.StudyRoomUser sru on sr.id = sru.studyRoomId
                //join sb.[User] u on sru.UserId = u.Id
                //where sr.Id in (select StudyRoomId from sb.StudyRoomUser where userid = @UserId)
                //and sru.UserId <> @UserId
                //order by COALESCE((select max(Created) from sb.StudyRoomSession where StudyRoomId = sr.Id), sr.[DateTime]) desc", new { query.UserId });
                //                    return result;

                //}

            }
        }
    }
}