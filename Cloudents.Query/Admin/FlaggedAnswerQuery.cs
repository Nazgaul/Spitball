﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class FlaggedAnswerQuery : IQueryAdmin2<IEnumerable<FlaggedAnswerDto>>
    {
        public Country? Country { get; }
        public FlaggedAnswerQuery(Country? country)
        {
            Country = country;
        }

        internal sealed class FlaggedAnswerEmptyQueryHandler : IQueryHandler<FlaggedAnswerQuery, IEnumerable<FlaggedAnswerDto>>
        {
            private readonly IStatelessSession _session;


            public FlaggedAnswerEmptyQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<FlaggedAnswerDto>> GetAsync(FlaggedAnswerQuery query, CancellationToken token)
            {
                var answers = _session.Query<Answer>()
                    .WithOptions(w => w.SetComment(nameof(FlaggedAnswerQuery)))
                    .Fetch(f => f.User)
                    .Where(w => w.Status.State == ItemState.Flagged);
                if (query.Country != null)
                {
                    answers = answers.Where(w => w.User.SbCountry == query.Country);
                }

                return await answers.Select(s => new FlaggedAnswerDto
                {
                    Id = s.Id,
                    Text = s.Text,
                    Reason = s.Status.FlagReason!,
                    FlaggedUserEmail = s.User.Email,
                    QuestionId = s.Question.Id,
                    QuestionText = s.Question.Text,
                    MarkerEmail = s.Status.FlaggedUser!.Email
                }).OrderBy(o => o.Id).ToListAsync(token);
            }
        }
    }
}
