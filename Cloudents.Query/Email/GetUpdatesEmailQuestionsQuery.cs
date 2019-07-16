using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Dapper;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailByUserQuery : IQuery<(IEnumerable<QuestionEmailDto>, IEnumerable<DocumentEmailDto>)>
    {
        public GetUpdatesEmailByUserQuery(long userId, DateTime since)
        {
            UserId = userId;
            Since = since;
        }
        private long UserId { get; }
        private DateTime Since { get; }

        internal sealed class GetUpdatesEmailQuestionsQueryHandler : IQueryHandler<GetUpdatesEmailByUserQuery, (IEnumerable<QuestionEmailDto>, IEnumerable<DocumentEmailDto>)>
        {
            private readonly IStatelessSession _session;

            public GetUpdatesEmailQuestionsQueryHandler(QuerySession querySession)
            {
                _session = querySession.StatelessSession;
            }

            public async Task<(IEnumerable<QuestionEmailDto>, IEnumerable<DocumentEmailDto>)> GetAsync(GetUpdatesEmailByUserQuery query, CancellationToken token)
            {
                User userAlias = null;
                QuestionEmailDto questionEmailDtoAlias = null;

                var queryCourse = QueryOver.Of<UserCourse>().Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Course.Id);

                var questionFuture = _session.QueryOver<Question>()
                     .JoinAlias(x => x.User, () => userAlias)
                     .Where(x => x.Created > query.Since)
                     .And(x => x.Status.State == ItemState.Ok)
                     .WithSubquery.WhereProperty(x => x.Course.Id).In(queryCourse)
                     .And(x => x.User.Id != query.UserId)


                     .SelectList(sl =>
                     {
                         sl.Select(() => userAlias.Id).WithAlias(() => questionEmailDtoAlias.UserId);
                         sl.Select(x => x.Id).WithAlias(() => questionEmailDtoAlias.QuestionId);
                         sl.Select(x => x.Text).WithAlias(() => questionEmailDtoAlias.QuestionText);
                         sl.Select(() => userAlias.Name).WithAlias(() => questionEmailDtoAlias.UserName);
                         sl.Select(() => userAlias.Image).WithAlias(() => questionEmailDtoAlias.UserImage);
                         return sl;
                     }).TransformUsing(Transformers.AliasToBean<QuestionEmailDto>())
                     .Future<QuestionEmailDto>();

                DocumentEmailDto documentEmailDtoAlias = null;


                var documentFuture = _session.QueryOver<Document>()
                    .JoinAlias(x => x.User, () => userAlias)
                    .Where(x => x.TimeStamp.CreationTime > query.Since)
                    .And(x => x.Status.State == ItemState.Ok)
                    .WithSubquery.WhereProperty(x => x.Course.Id).In(queryCourse)
                    .And(x => x.User.Id != query.UserId)


                    .SelectList(sl =>
                    {
                        sl.Select(x => x.Id).WithAlias(() => documentEmailDtoAlias.Id);
                        sl.Select(x => x.Name).WithAlias(() => documentEmailDtoAlias.Name);
                        sl.Select(() => userAlias.Name).WithAlias(() => documentEmailDtoAlias.UserName);
                        return sl;
                    }).TransformUsing(Transformers.AliasToBean<DocumentEmailDto>())
                    .Future<DocumentEmailDto>();

                var questions = await questionFuture.GetEnumerableAsync(token);
                var documents = await documentFuture.GetEnumerableAsync(token);

                return (questions, documents);
            }
        }
    }
}
