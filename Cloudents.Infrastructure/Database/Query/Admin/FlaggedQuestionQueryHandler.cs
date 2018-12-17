﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    internal class FlaggedQuestionQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<FlaggedQuestionDto>>
    {

        private readonly IStatelessSession _session;


        public FlaggedQuestionQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<FlaggedQuestionDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            return await _session.Query<Question>()
                .Fetch(f => f.User)
                .Where(w => w.User is RegularUser && w.Item.State == ItemState.Flagged)
                .Select(s => new FlaggedQuestionDto
                {
                    Id = s.Id,
                    Reason = s.Item.FlagReason,
                    FlaggedUserEmail = s.Item.FlaggedUser.Email
                }).OrderBy(o => o.Id).ToListAsync(token);
        }
    }
}
