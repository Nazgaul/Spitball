﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    [UsedImplicitly]
    public class DocumentByIdQueryHandler : IQueryHandler<DocumentById, DocumentDetailDto>
    {
        private readonly IStatelessSession _session;

        public DocumentByIdQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public Task<DocumentDetailDto> GetAsync(DocumentById query, CancellationToken token)
        {
            return _session.Query<Document>()
                
                .Fetch(f=>f.University)
                .Fetch(f=>f.User)
                .Where(w => w.Id == query.Id && w.Item.State == ItemState.Ok)
                .Select(s => new DocumentDetailDto
                {
                    Name = s.Name,
                    Date = s.TimeStamp.UpdateTime,
                   // Blob = s.BlobName,
                    University = s.University.Name,
                    TypeStr =  s.Type,
                    Pages = s.PageCount.GetValueOrDefault(),
                    Professor = s.Professor,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    
                    User = new UserDto(s.User.Id,s.User.Name, s.User.Score),
                    //{
                    //    Id = s.User.Id,
                    //    Name = s.User.Name,
                    //    Image = s.User.Image,
                    //    Score = s.User.Score
                    //},
                    Course = s.Course.Name
                })
                .SingleOrDefaultAsync(token);
        }
    }
}