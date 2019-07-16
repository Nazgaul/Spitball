using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailUsersQuery : IQuery<IEnumerable<UpdateEmailDto>>
    {
        public GetUpdatesEmailUsersQuery(DateTime since, int page)
        {
            Since = since;
            Page = page;
        }

        private int Page { get; }
        private DateTime Since { get; }

        internal sealed class GetUpdatesEmailUsersQueryHandler : IQueryHandler<GetUpdatesEmailUsersQuery, IEnumerable<UpdateEmailDto>>
        {
            private readonly IStatelessSession _session;

            public GetUpdatesEmailUsersQueryHandler(QuerySession querySession)
            {
                _session = querySession.StatelessSession;
            }

            //public static QueryOver<Question> GetQuestionData(DateTime since)
            //{
            //    Question questionAlias = null;
            //    UserCourse userCourseAlias = null;

            //    return QueryOver.Of(() => questionAlias)
            //          .JoinEntityAlias(() => userCourseAlias, () => questionAlias.Course.Id == userCourseAlias.Course.Id)
            //          .Where(w => w.Created > since)
            //          .And(x => x.Status.State == ItemState.Ok);
            //}

            public async Task<IEnumerable<UpdateEmailDto>> GetAsync(GetUpdatesEmailUsersQuery query, CancellationToken token)
            {
                Question questionAlias = null;
                Document documentAlias = null;
                UserCourse userCourseAlias = null;
                User userAlias = null;

                UpdateEmailDto updateEmailAlias = null;


                var queryQuestion = QueryOver.Of(() => questionAlias)
                     .JoinEntityAlias(() => userCourseAlias, () => questionAlias.Course.Id == userCourseAlias.Course.Id)
                     .Where(w => w.Created > query.Since)
                     .And(x => x.Status.State == ItemState.Ok)
                     .Select(s => s.User.Id);


                var queryDocument = QueryOver.Of(() => documentAlias)
                    .JoinEntityAlias(() => userCourseAlias, () => documentAlias.Course.Id == userCourseAlias.Course.Id)
                    .Where(w => w.TimeStamp.CreationTime > query.Since)
                    .And(x => x.Status.State == ItemState.Ok)
                    .Select(s => s.User.Id);


                return await
                     _session.QueryOver(() => userAlias)
                         .Where(w => w.LockoutEnd < DateTimeOffset.UtcNow)
                         .And(Restrictions.Disjunction()
                             .Add(Subqueries.WhereProperty(() => userAlias.Id).In(queryQuestion))
                             .Add(Subqueries.WhereProperty(() => userAlias.Id).In(queryDocument))

 )
                     .SelectList(sl =>
                     {
                         sl.Select(() => userAlias.Id).WithAlias(() => updateEmailAlias.UserId);
                         sl.Select(() => userAlias.Email).WithAlias(() => updateEmailAlias.ToEmailAddress);
                         sl.Select(() => userAlias.Language).WithAlias(() => updateEmailAlias.Language);
                         sl.Select(() => userAlias.Name).WithAlias(() => updateEmailAlias.UserName);
                         return sl;
                     }).TransformUsing(Transformers.AliasToBean<UpdateEmailDto>())
                         .OrderBy(x => x.Id).Asc
                     .Take(100).Skip(100 * query.Page).ListAsync<UpdateEmailDto>(token);




                //                const string sql = @"
                //select u.Id, u.[Name] as UserName, u.Email as 'To'
                //from sb.Question q
                //join sb.UsersCourses uc
                //	on q.CourseId = uc.CourseId
                //join sb.[User] u
                //	on uc.UserId = u.Id
                //where q.Created > getutcdate() - 10
                //union
                //select u.Id, u.[Name] as UserName, u.Email as 'To'
                //from sb.Document q
                //join sb.UsersCourses uc
                //	on q.CourseName = uc.CourseId
                //join sb.[User] u
                //	on uc.UserId = u.Id
                //where q.CreationTime > getutcdate() - 10
                //order by u.Id";

                //                using (var connection = _dapperRepository.OpenConnection())
                //                {
                //                    return await connection.QueryAsync<UpdateEmailDto>(sql);
                //                }
            }
        }
    }
}
