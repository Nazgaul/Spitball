﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class FlaggedAnswerQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<FlaggedAnswerDto>>
    {

        private readonly IStatelessSession _session;


        public FlaggedAnswerQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<FlaggedAnswerDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Answer>()
                .Fetch(f => f.User)
                .Where(w => w.Item.State == ItemState.Flagged)
                .Select(s => new FlaggedAnswerDto
                {
                    Id = s.Id,
                    Reason = s.Item.FlagReason,
                    FlaggedUserEmail = s.Item.FlaggedUser.Email
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
