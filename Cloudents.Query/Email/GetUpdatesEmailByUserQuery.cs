﻿using Cloudents.Core.DTOs.Email;
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

            public GetUpdatesEmailQuestionsQueryHandler(IStatelessSession querySession)
            {
                _session = querySession;
            }

            public async Task<IEnumerable<UpdateEmailDto>> GetAsync(GetUpdatesEmailByUserQuery query, CancellationToken token)
            {
                User userAlias = null!;
                //Question questionAlias = null!;
                //QuestionUpdateEmailDto questionEmailDtoAlias = null!;

                var followCourse = QueryOver.Of<Follow>().Where(w => w.Follower.Id == query.UserId)
                    .Select(s => s.User.Id);

                //var firstAnswer = QueryOver.Of<Answer>().Where(w => w.Question.Id == questionAlias.Id)
                //    .Where(w => w.Status.State == ItemState.Ok)
                //    .Select(s => s.Text)
                //    .Take(1);


                //var queryCountry = QueryOver.Of<User>().Where(w => w.Id == query.UserId)
                //    .Select(s => s.Country);

                //var questionFuture = _session.QueryOver(() => questionAlias)
                //     .JoinAlias(x => x.User, () => userAlias)
                //     .Where(x => x.Created > query.Since)
                //     .And(x => x.Status.State == ItemState.Ok)
                //     .WithSubquery.WhereProperty(x => x.User.Id).In(followCourse)
                //     //.WithSubquery.WhereProperty(() => userAlias.Country).Eq(queryCountry)
                //     .And(x => x.User.Id != query.UserId)


                //     .SelectList(sl =>
                //     {
                //         sl.Select(() => userAlias.Id).WithAlias(() => questionEmailDtoAlias.UserId);
                //         sl.Select(x => x.Id).WithAlias(() => questionEmailDtoAlias.QuestionId);
                //         sl.Select(x => x.Text).WithAlias(() => questionEmailDtoAlias.QuestionText);
                //         sl.Select(() => userAlias.Name).WithAlias(() => questionEmailDtoAlias.UserName);
                //         sl.Select(() => userAlias.ImageName).WithAlias(() => questionEmailDtoAlias.UserImage);
                //         sl.Select(x => x.Course.Id).WithAlias(() => questionEmailDtoAlias.Course);
                //         sl.SelectSubQuery(firstAnswer).WithAlias(() => questionEmailDtoAlias.AnswerText);
                //         return sl;
                //     }).TransformUsing(Transformers.AliasToBean<QuestionUpdateEmailDto>())
                //     .UnderlyingCriteria.SetComment(nameof(GetUpdatesEmailByUserQuery))
                //     .Future<QuestionUpdateEmailDto>();

                DocumentUpdateEmailDto documentEmailDtoAlias = null!;


                var documentFuture = _session.QueryOver<Document>()
                    .JoinAlias(x => x.User, () => userAlias)
                    .Where(x => x.TimeStamp.CreationTime > query.Since)
                    .And(x => x.Status.State == ItemState.Ok)
                    .WithSubquery.WhereProperty(x => x.User.Id).In(followCourse)
                    //.WithSubquery.WhereProperty(() => userAlias.Country).Eq(queryCountry)
                    .And(x => x.User.Id != query.UserId)


                    .SelectList(sl =>
                    {
                        sl.Select(x => x.Id).WithAlias(() => documentEmailDtoAlias.Id);
                        sl.Select(x => x.Name).WithAlias(() => documentEmailDtoAlias.Name);
                        sl.Select(() => userAlias.Name).WithAlias(() => documentEmailDtoAlias.UserName);
                        sl.Select(() => userAlias.Id).WithAlias(() => documentEmailDtoAlias.UserId);
                        sl.Select(x => x.OldCourse.Id).WithAlias(() => documentEmailDtoAlias.Course);
                        sl.Select(() => userAlias.ImageName).WithAlias(() => documentEmailDtoAlias.UserImage);
                        sl.Select(x => x.DocumentType).WithAlias(() => documentEmailDtoAlias.DocumentType);
                        return sl;
                    }).TransformUsing(Transformers.AliasToBean<DocumentUpdateEmailDto>())
                    .OrderBy(x=>x.DocumentType).Desc
                    .UnderlyingCriteria.SetComment(nameof(GetUpdatesEmailByUserQuery))
                    .Future<DocumentUpdateEmailDto>();

                //IEnumerable<UpdateEmailDto> questions = await questionFuture.GetEnumerableAsync(token);
                IEnumerable<UpdateEmailDto> documents = await documentFuture.GetEnumerableAsync(token);

                return documents;
            }
        }
    }
}
