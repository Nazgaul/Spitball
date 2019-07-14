﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class FlaggedAnswerEmptyQuery : IQuery<IEnumerable<FlaggedAnswerDto>>
    {
        internal sealed class FlaggedAnswerEmptyQueryHandler : IQueryHandler<FlaggedAnswerEmptyQuery, IEnumerable<FlaggedAnswerDto>>
        {
            private readonly IStatelessSession _session;


            public FlaggedAnswerEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<FlaggedAnswerDto>> GetAsync(FlaggedAnswerEmptyQuery query, CancellationToken token)
            {
                return await _session.Query<Answer>()
                    .Fetch(f => f.User)
                    .Where(w => w.Status.State == ItemState.Flagged)
                    .Select(s => new FlaggedAnswerDto
                    {
                        Id = s.Id,
                        Text = s.Text,
                        Reason = s.Status.FlagReason,
                        FlaggedUserEmail = s.User.Email,
                        QuestionId = s.Question.Id,
                        QuestionText = s.Question.Text,
                        MarkerEmail = s.Status.FlaggedUser.Email
                    }).OrderBy(o => o.Id).ToListAsync(token);
            }
        }
    }
}
