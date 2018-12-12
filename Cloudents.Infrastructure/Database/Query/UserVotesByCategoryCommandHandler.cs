﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    public class UserVotesByCategoryCommandHandler : 
        IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteDocumentDto>>,
        IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteQuestionDto>>,
        IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteAnswerDto>>
    {
        private readonly IStatelessSession _session;

        public UserVotesByCategoryCommandHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        
        async Task<IEnumerable<UserVoteDocumentDto>> IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteDocumentDto>>.GetAsync(UserVotesByCategoryQuery query, CancellationToken token)
        {
            return await _session.Query<Vote>()
                    .Where(w => w.Document != null)
                    .Select(s => new UserVoteDocumentDto
                    {
                        Id = s.Document.Id,
                        Vote = s.VoteType
                    }).ToListAsync(token);
        }

        async Task<IEnumerable<UserVoteQuestionDto>> IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteQuestionDto>>.GetAsync(UserVotesByCategoryQuery query, CancellationToken token)
        {
            return await _session.Query<Vote>()
                    .Where(w => w.Question != null && w.Answer != null)
                    .Select(s => new UserVoteQuestionDto
                    {
                        Id = s.Question.Id,
                        Vote = s.VoteType
                    }).ToListAsync(token);
        }

        async Task<IEnumerable<UserVoteAnswerDto>> IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteAnswerDto>>.GetAsync(UserVotesByCategoryQuery query, CancellationToken token)
        {
            return await _session.Query<Vote>()
                .Where(w => w.Question != null && w.Answer != null)
                .Select(s => new UserVoteAnswerDto
                {
                    Id = s.Answer.Id,
                    QuestionId = s.Question.Id,
                    Vote = s.VoteType
                }).ToListAsync(token);
        }
    }
}