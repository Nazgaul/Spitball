using Cloudents.Core.DTOs.Email;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.SqlCommand;

namespace Cloudents.Query.Email
{
    public class GetUpdatesEmailByUserQuery : IQuery<IEnumerable<UpdateEmailDto>>
    {
        public GetUpdatesEmailByUserQuery(long userId, DateTime since)
        {
            UserId = userId;
            Since = since;
        }
        private long UserId { get; }
        private DateTime Since { get; }

        internal sealed class GetUpdatesEmailQuestionsQueryHandler : IQueryHandler<GetUpdatesEmailByUserQuery, IEnumerable<UpdateEmailDto>>
        {
            private readonly IStatelessSession _session;

            public GetUpdatesEmailQuestionsQueryHandler(QuerySession querySession)
            {
                _session = querySession.StatelessSession;
            }

            public async Task<IEnumerable<UpdateEmailDto>> GetAsync(GetUpdatesEmailByUserQuery query, CancellationToken token)
            {
                User? userAlias = null;
                Question? questionAlias = null;
                QuestionUpdateEmailDto? questionEmailDtoAlias = null;
                Course2 courseAlias = null;

                var queryCourse = QueryOver.Of<UserCourse2>().Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Course.Id);

                var firstAnswer = QueryOver.Of<Answer>().Where(w => w.Question.Id == questionAlias.Id)
                    .Where(w => w.Status.State == ItemState.Ok)
                    .Select(s => s.Text)
                    .Take(1);


                var questionFuture = _session.QueryOver(() => questionAlias)
                     .JoinAlias(x => x.User, () => userAlias)
                     .JoinAlias(x => x.Course2, () => courseAlias, JoinType.LeftOuterJoin)
                     .Where(x => x.Created > query.Since)
                     .And(x => x.Status.State == ItemState.Ok)
                     .WithSubquery.WhereProperty(x => x.Course2.Id).In(queryCourse)
                     .And(x => x.User.Id != query.UserId)


                     .SelectList(sl =>
                     {
                         sl.Select(() => userAlias.Id).WithAlias(() => questionEmailDtoAlias.UserId);
                         sl.Select(x => x.Id).WithAlias(() => questionEmailDtoAlias.QuestionId);
                         sl.Select(x => x.Text).WithAlias(() => questionEmailDtoAlias.QuestionText);
                         sl.Select(() => userAlias.Name).WithAlias(() => questionEmailDtoAlias.UserName);
                         sl.Select(() => userAlias.ImageName).WithAlias(() => questionEmailDtoAlias.UserImage);
                         sl.Select(() => courseAlias.CardDisplay).WithAlias(() => questionEmailDtoAlias.Course);
                         sl.SelectSubQuery(firstAnswer).WithAlias(() => questionEmailDtoAlias.AnswerText);
                         return sl;
                     }).TransformUsing(Transformers.AliasToBean<QuestionUpdateEmailDto>())
                     .UnderlyingCriteria.SetComment(nameof(GetUpdatesEmailByUserQuery))
                     .Future<QuestionUpdateEmailDto>();

                DocumentUpdateEmailDto documentEmailDtoAlias = null;


                var documentFuture = _session.QueryOver<Document>()
                    .JoinAlias(x => x.User, () => userAlias)
                    .JoinAlias(x => x.Course2, () => courseAlias)
                    .Where(x => x.TimeStamp.CreationTime > query.Since)
                    .And(x => x.Status.State == ItemState.Ok)
                    .WithSubquery.WhereProperty(x => x.Course2.Id).In(queryCourse)
                    .And(x => x.User.Id != query.UserId)


                    .SelectList(sl =>
                    {
                        sl.Select(x => x.Id).WithAlias(() => documentEmailDtoAlias.Id);
                        sl.Select(x => x.Name).WithAlias(() => documentEmailDtoAlias.Name);
                        sl.Select(() => userAlias.Name).WithAlias(() => documentEmailDtoAlias.UserName);
                        sl.Select(() => userAlias.Id).WithAlias(() => documentEmailDtoAlias.UserId);
                        sl.Select(() => courseAlias.CardDisplay).WithAlias(() => documentEmailDtoAlias.Course);
                        sl.Select(() => userAlias.ImageName).WithAlias(() => documentEmailDtoAlias.UserImage);
                        sl.Select(x => x.DocumentType).WithAlias(() => documentEmailDtoAlias.DocumentType);
                        return sl;
                    }).TransformUsing(Transformers.AliasToBean<DocumentUpdateEmailDto>())
                    .OrderBy(x => x.DocumentType).Desc
                    .UnderlyingCriteria.SetComment(nameof(GetUpdatesEmailByUserQuery))
                    .Future<DocumentUpdateEmailDto>();

                IEnumerable<UpdateEmailDto> questions = await questionFuture.GetEnumerableAsync(token);
                IEnumerable<UpdateEmailDto> documents = await documentFuture.GetEnumerableAsync(token);

                return questions.Union(documents);
            }
        }
    }
}
