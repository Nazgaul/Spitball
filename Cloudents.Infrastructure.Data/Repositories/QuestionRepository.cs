using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [UsedImplicitly]
    public class QuestionRepository : NHibernateRepository<Question>, IQuestionRepository
    {
        //public QuestionRepository(IIndex<Core.Enum.Database, IUnitOfWork> unitOfWork) : base(unitOfWork)
        //{
        //}

        public QuestionRepository(ISession session) : base(session)
        {
        }

       

        [SuppressMessage("ReSharper", "CoVariantArrayConversion", Justification = "Is in can get all of the types")]
        [SuppressMessage("Microsoft", "CC0030", Justification = "We can't do that nhibernate uses those objects")]
        public async Task<ResultWithFacetDto<QuestionDto>> GetQuestionsAsync(QuestionsQuery query, CancellationToken token)
        {
            QuestionDto dto = null;
            QuestionSubject commentAlias = null;
            Question questionAlias = null;
            User userAlias = null;

            var queryOverObj = Session.QueryOver(() => questionAlias)
                .JoinAlias(x => x.Subject, () => commentAlias)
                .JoinAlias(x => x.User, () => userAlias)
                .SelectList(l => l
                    .Select(_ => commentAlias.Text).WithAlias(() => dto.Subject)
                    .Select(s => s.Id).WithAlias(() => dto.Id)
                    .Select(s => s.Text).WithAlias(() => dto.Text)
                    .Select(s => s.Price).WithAlias(() => dto.Price)
                    .Select(s => s.Attachments).WithAlias(() => dto.Files)
                    .Select(s => s.Created).WithAlias(() => dto.DateTime)
                    //.SelectCount(s => s.Answers).WithAlias(() => dto.Answers)
                    //.Select(_ => userAlias.Name).As("User.Name")
                    .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                    .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                    .Select(Projections.Property(() => userAlias.Image).As("User.Image"))
                    .SelectSubQuery(QueryOver.Of<Answer>()
                        .Where(w => w.Question.Id == questionAlias.Id).ToRowCountQuery()).WithAlias(() => dto.Answers)

                )
                .Where(w => w.CorrectAnswer == null)
                .TransformUsing(new DeepTransformer<QuestionDto>());
            if (query.Source != null)
            {
                queryOverObj.WhereRestrictionOn(() => commentAlias.Text).IsIn(query.Source);
            }

            if (!string.IsNullOrEmpty(query.Term))
            {
                queryOverObj.Where(new FullTextCriterion(Projections.Property<Question>(x => x.Text),
                    query.Term));
            }

            queryOverObj.OrderBy(o => o.Id).Desc
                .Skip(query.Page * 50)
                .Take(50);

            var futureQueryOver = queryOverObj.Future<QuestionDto>();

            var facetsFuture = Session.Query<QuestionSubject>().Select(s => s.Text).ToFuture();
            var retVal = await futureQueryOver.GetEnumerableAsync(token).ConfigureAwait(false);
            var facet = await facetsFuture.GetEnumerableAsync(token).ConfigureAwait(false);

            return new ResultWithFacetDto<QuestionDto>
            {
                Result = retVal,
                Facet = facet
            };
        }

        //public async Task<QuestionDetailDto> GetQuestionDtoAsync(long id, CancellationToken token)
        //{
        //    //TODO: this is left join query need to fix that
        //    var questionFuture = Session.Query<Question>().Where(w => w.Id == id)
        //        .Fetch(f => f.Subject)
        //        .Fetch(f => f.User)
        //        .Select(s => new QuestionDetailDto
        //        {
        //            User = new UserDto
        //            {
        //                Id = s.User.Id,
        //                Name = s.User.Name,
        //                Image = s.User.Image
        //            },
        //            Id = s.Id,
        //            Create = s.Created,
        //            Price = s.Price,
        //            Subject = s.Subject.Text,
        //            Text = s.Text,
        //            CorrectAnswerId = s.CorrectAnswer.Id
        //        }).ToFutureValue();
        //    var answersFuture = Session.Query<Answer>()
        //        .Where(w => w.Question.Id == id)
        //        .Fetch(f => f.User)
        //        .Select(s => new QuestionDetailAnswerDto
        //        {
        //            Id = s.Id,
        //            Text = s.Text,
        //            Create = s.Created,
        //            User = new UserDto
        //            {
        //                Id = s.User.Id,
        //                Name = s.User.Name,
        //                Image = s.User.Image
        //            }
        //        }).ToFuture();

        //    var dto = await questionFuture.GetValueAsync(token).ConfigureAwait(false);
        //    if (dto == null)
        //    {
        //        return null;
        //    }
        //    dto.Answers = await answersFuture.GetEnumerableAsync(token).ConfigureAwait(false);

        //    return dto;
        //}

        public async Task<IList<Question>> GetAllQuestionsAsync()
        {
            var t = await Session.Query<Question>().ToListAsync();
            return t;
        }
    }
}