﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using JetBrains.Annotations;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [UsedImplicitly]
    public class QuestionRepository : NHibernateRepository<Question>, IQuestionRepository
    {
        public QuestionRepository(IIndex<Core.Enum.Database, IUnitOfWork> unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<ResultWithFacetDto<QuestionDto>> GetQuestionsAsync(QuestionsQuery query, CancellationToken token)
        {
            QuestionDto dto = null;
            QuestionSubject commentAlias = null;
            var queryOverObj = Session.QueryOver<Question>()
                .JoinAlias(x => x.Subject, () => commentAlias)
                .SelectList(l => l
                        .Select(_ => commentAlias.Text).WithAlias(() => dto.Subject)
                        .Select(s => s.Id).WithAlias(() => dto.Id)
                        .Select(s => s.Text).WithAlias(() => dto.Text)
                        .Select(s => s.Price).WithAlias(() => dto.Price)
                )
                .Where(w => w.CorrectAnswer == null)
                .TransformUsing(Transformers.AliasToBean<QuestionDto>());
            if (query.Source != null)
            {
                queryOverObj.WhereRestrictionOn(() => commentAlias.Text).IsIn(query.Source);
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

        public async Task<QuestionDetailDto> GetQuestionDtoAsync(long id, CancellationToken token)
        {
            /* QuestionDetailDto dto = null;
             //QuestionDetailUserDto userDto = null;
             QuestionSubject commentAlias = null;
             Answer answerAlias = null;
             User userAlias = null;
             var rootObj = Session.QueryOver<Question>().Where(w => w.Id == id)
                 .JoinAlias(x => x.Subject, () => commentAlias)
                 .JoinAlias(x => x.User, () => userAlias)
                 //.Left.JoinAlias(x => x.Answers, () => answerAlias)
                 .SelectList(l => l
                     .Select(_ => commentAlias.Subject).WithAlias(() => dto.Subject)

                     .Select(Projections.Property(() => userAlias.Id).As("User.Id"))
                     .Select(Projections.Property(() => userAlias.Name).As("User.Name"))
                     //.Select(_ => userAlias.Id).WithAlias(() => userDto.Id)
                     //.Select(_ => userAlias.Name).WithAlias(() => userDto.Name)

                     .Select(s => s.Id).WithAlias(() => dto.Id)
                     .Select(s => s.Text).WithAlias(() => dto.Text)
                     .Select(s => s.Price).WithAlias(() => dto.Price)
                     .Select(s => s.Created).WithAlias(() => dto.Create)
                 //.Select(Projections.Property(() => answerAlias.Id).As("Answers.Id"))
                 )

                 //.Fetch(f => f.Answers).Eager
                 .TransformUsing(new DeepTransformer<QuestionDetailDto>())
                 .FutureValue<QuestionDetailDto>();*/

            //TODO: this is left join query need to fix that
            var questionFuture = Session.Query<Question>().Where(w => w.Id == id)
                .Fetch(f => f.Subject)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailDto
                {
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image
                    },
                    Id = s.Id,
                    Create = s.Created,
                    Price = s.Price,
                    Subject = s.Subject.Text,
                    Text = s.Text,
                    CorrectAnswerId = s.CorrectAnswer.Id
                }).ToFutureValue();
            var answersFuture = Session.Query<Answer>()
                .Where(w => w.Question.Id == id)
                .Fetch(f => f.User)
                .Select(s => new QuestionDetailAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Create = s.Created,
                    UpVote = s.UpVote.GetValueOrDefault(),
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image
                    }
                }).ToFuture();

            var dto = await questionFuture.GetValueAsync(token).ConfigureAwait(false);
            if (dto == null)
            {
                return null;
            }
            dto.Answers = await answersFuture.GetEnumerableAsync(token).ConfigureAwait(false);

            return dto;
        }
    }
}