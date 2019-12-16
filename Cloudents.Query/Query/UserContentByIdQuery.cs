using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserContentByIdQuery : IQuery<IEnumerable<UserContentDto>>
    {
        public UserContentByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }

        internal sealed class UserContentByIdQueryHandler : IQueryHandler<UserContentByIdQuery, IEnumerable<UserContentDto>>
        {
            private readonly IStatelessSession _session;

            public UserContentByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserContentDto>> GetAsync(UserContentByIdQuery query, CancellationToken token)
            {
                var documentFuture = _session.Query<Document>()
                    .Fetch(f => f.User).FetchMany(f => f.Transactions)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => new UserDocumentsDto()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Course = s.Course.Id,
                        Type = s.DocumentType != null?  s.DocumentType.ToString() : "Document",
                        Likes = s.VoteCount,
                        Price = s.Price,
                        State = s.Status.State,
                        Date = s.TimeStamp.CreationTime,
                        Views = s.Views,
                        Downloads = s.Downloads,
                        Purchased = s.Transactions.Where(w => w is DocumentTransaction).Count()
                    }).ToFuture<UserContentDto>();


                var questionFuture = _session.Query<Question>()
                    .FetchMany(f => f.Answers)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => new UserQuestionsDto()
                    {
                        Id = s.Id,
                        State = s.Status.State,
                        Date = s.Created,
                        Course = s.Course.Id,
                        Text = s.Text,
                        AnswerText = s.Answers.Select(si => si.Text).FirstOrDefault()
                    }).ToFuture<UserContentDto>();

                var answerFuture = _session.Query<Answer>()
                    .Fetch(f => f.User).Fetch(f => f.Question)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => new UserAnswersDto()
                    { 
                        QuestionId = s.Question.Id,
                        State = s.Status.State,
                        Date = s.Created,
                        Course = s.Question.Course.Id,
                        QuestionText = s.Question.Text,
                        AnswerText = s.Text

                    }).ToFuture<UserContentDto>();

                IEnumerable<UserContentDto> documentResult = await documentFuture.GetEnumerableAsync(token);
                IEnumerable<UserContentDto> questionResult = await questionFuture.GetEnumerableAsync(token);
                IEnumerable<UserContentDto> answerResult = await answerFuture.GetEnumerableAsync(token);

                return documentResult.Union(questionResult).Union(answerResult).OrderByDescending(o => o.Date);
            }
        }
    }
}
