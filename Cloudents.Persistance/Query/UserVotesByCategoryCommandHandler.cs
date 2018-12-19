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

namespace Cloudents.Infrastructure.Database.Query
{
    public class UserVotesByCategoryCommandHandler :
        IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteDocumentDto>>,
        IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteQuestionDto>>,
        IQueryHandler<UserVotesQuestionQuery, IEnumerable<UserVoteAnswerDto>>
    {
        private readonly IStatelessSession _session;

        public UserVotesByCategoryCommandHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        async Task<IEnumerable<UserVoteDocumentDto>> IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteDocumentDto>>.GetAsync(UserVotesByCategoryQuery query, CancellationToken token)
        {
            return await _session.Query<Vote>()
                    .Where(w => w.Document != null && w.User.Id == query.UserId)
                    .Select(s => new UserVoteDocumentDto
                    {
                        Id = s.Document.Id,
                        Vote = s.VoteType
                    }).ToListAsync(token);
        }

        async Task<IEnumerable<UserVoteQuestionDto>> IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteQuestionDto>>.GetAsync(UserVotesByCategoryQuery query, CancellationToken token)
        {
            return await _session.Query<Vote>()
                    .Where(w => w.Question != null && w.Answer == null && w.User.Id == query.UserId)
                    .Select(s => new UserVoteQuestionDto
                    {
                        Id = s.Question.Id,
                        Vote = s.VoteType
                    }).ToListAsync(token);
        }

        async Task<IEnumerable<UserVoteAnswerDto>> IQueryHandler<UserVotesQuestionQuery,
            IEnumerable<UserVoteAnswerDto>>.GetAsync(UserVotesQuestionQuery query, CancellationToken token)
        {
            return await _session.Query<Vote>()
                .Where(w => w.Question.Id == query.QuestionId && w.User.Id == query.UserId)
                .Select(s => new UserVoteAnswerDto(s.Answer.Id,s.VoteType)
                ).ToListAsync(token);
        }
    }
}