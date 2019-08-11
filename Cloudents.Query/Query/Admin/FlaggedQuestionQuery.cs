﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Cloudents.Core.Enum;
using NHibernate.Linq;

namespace Cloudents.Query.Query.Admin
{
    public class FlaggedQuestionQuery : IQueryAdmin<IEnumerable<FlaggedQuestionDto>>
    {
        public FlaggedQuestionQuery(string country)
        {
            Country = country;
        }
        public string Country { get; }
        internal sealed class FlaggedQuestionEmptyQueryHandler : IQueryHandler<FlaggedQuestionQuery, IEnumerable<FlaggedQuestionDto>>
        {
            private readonly IStatelessSession _session;


            public FlaggedQuestionEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<FlaggedQuestionDto>> GetAsync(FlaggedQuestionQuery query, CancellationToken token)
            {
                var questions = _session.Query<Question>()
                    .Fetch(f => f.User)
                    .Where(w => w.User is User && w.Status.State == ItemState.Flagged);

                if (!string.IsNullOrEmpty(query.Country))
                {
                    questions = questions.Where(w => w.User.Country == query.Country);
                }
                    
                    return await questions.Select(s => new FlaggedQuestionDto
                    {
                        Id = s.Id,
                        Reason = s.Status.FlagReason,
                        Text = s.Text,
                        FlaggedUserEmail = s.Status.FlaggedUser.Email
                    }).OrderBy(o => o.Id).ToListAsync(token);
            }
        }
    }
}
