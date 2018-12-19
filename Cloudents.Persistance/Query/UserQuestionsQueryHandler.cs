using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query
{
    public class UserQuestionsQueryHandler : IQueryHandler<UserDataPagingByIdQuery, IEnumerable<QuestionFeedDto>>
    {
        private readonly ISession _session;

        public UserQuestionsQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }
        public async Task<IEnumerable<QuestionFeedDto>> GetAsync(UserDataPagingByIdQuery query, CancellationToken token)
        {
            return await _session.Query<Question>()
                .Where(w => w.User.Id == query.Id && w.Item.State == ItemState.Ok)
                .OrderByDescending(o => o.Id)
                .Select(s => new QuestionFeedDto(s.Id,
                    s.Subject,
                    s.Price,
                    s.Text,
                    s.Attachments,
                    s.AnswerCount,
                    s.Updated,
                    s.Color, s.CorrectAnswer.Id != null, s.Language, s.Item.VoteCount)
                )
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }
}
