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
    public class DocumentsQueryHandler : IQueryHandler<IdsQuery<long>, IList<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public DocumentsQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IList<DocumentFeedDto>> GetAsync(IdsQuery<long> query, CancellationToken token)
        {
            var ids = query.QuestionIds.ToList();
            return await _session.Query<Document>()
                .Fetch(f => f.User)
                .Where(w => ids.Contains(w.Id))
                .Select(s => new DocumentFeedDto
                {
                    Id = s.Id,
                    User = new UserDto()
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image
                    },
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Name,
                    TypeStr = s.Type,
                    Professor = s.Professor,
                    Title = s.Name,
                    Views = s.Views,
                    University = s.University.Name
                })
                .ToListAsync(token);

        }
    }
}