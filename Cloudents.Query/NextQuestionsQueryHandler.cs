//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Query.Query;
//using Cloudents.Query.Stuff;
//using NHibernate;
//using NHibernate.Criterion;

//namespace Cloudents.Query
//{
//    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
//    public class NextQuestionsQueryHandler : IQueryHandler<NextQuestionQuery, IEnumerable<QuestionFeedDto>>
//    {
//        private readonly IStatelessSession _session;

//        public NextQuestionsQueryHandler(QuerySession session)
//        {
//            _session = session.StatelessSession;
//        }

//        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(NextQuestionQuery query, CancellationToken token)
//        {
//            QuestionFeedDto dto = null;
//            Question questionAlias = null;
//            BaseUser userAlias = null;


//            var detachedQuery = QueryOver.Of<Question>()
//                .Select(s => s.Course.Id)
//                .Where(w => w.Id == query.QuestionId && w.Status.State == ItemState.Ok)
//                .Take(1);

//            return await _session.QueryOver(() => questionAlias)
//                .JoinAlias(x => x.User, () => userAlias)
//                .SelectList(l => l
//                    //.Select(s => s.Subject).WithAlias(() => dto.Subject)
//                    .Select(s => s.Id).WithAlias(() => dto.Id)
//                    .Select(s => s.Text).WithAlias(() => dto.Text)
//                    .Select(s => s.Attachments).WithAlias(() => dto.Files)
//                    .Select(s => s.Updated).WithAlias(() => dto.DateTime)
//                    .Select(s => s.Language).WithAlias(() => dto.CultureInfo)
//                    .Select(s => s.Course.Id).WithAlias(() => dto.Course)
//                    .Select(Projections.Property(() => questionAlias.VoteCount).As("Vote.Votes"))
//                    .SelectSubQuery(QueryOver.Of<Answer>()
//                        .Where(w => w.Question.Id == questionAlias.Id && w.Status.State == ItemState.Ok).ToRowCountQuery()).WithAlias(() => dto.Answers)
//                    .Select(Projections.Conditional(
//                        Restrictions.Where(() => questionAlias.CorrectAnswer != null),
//                        Projections.Constant(true), Projections.Constant(false))).WithAlias(() => dto.HasCorrectAnswer)
//                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
//                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
//                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))

//                )
//                .Where(w => w.CorrectAnswer == null)
//                .Where(w => w.User.Id != query.UserId)
//                .Where(w => w.Id != query.QuestionId)
//                .Where(w => w.Status.State == ItemState.Ok)
//                .WithSubquery.WhereProperty(x => x.Id)
//                .NotIn(QueryOver.Of<Answer>().
//                    Where(w => w.User.Id == query.UserId && w.Status.State == ItemState.Ok).Select(s => s.Question.Id))
//                .TransformUsing(new DeepTransformer<QuestionFeedDto>())
//                .OrderBy(Projections.Conditional(
//                    Subqueries.PropertyEq(nameof(Question.Course), detachedQuery.DetachedCriteria)
//                    , Projections.Constant(0), Projections.Constant(1)
//                    )).Asc
//                .ThenBy(Projections.SqlFunction("random_Order", NHibernateUtil.Guid)).Asc
//                .Take(3).ListAsync<QuestionFeedDto>(token);
//        }
//    }
//}