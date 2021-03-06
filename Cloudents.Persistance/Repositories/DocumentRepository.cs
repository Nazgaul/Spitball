﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class DocumentRepository : NHibernateRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(ISession session) : base(session)
        {


        }
        public Task UpdateNumberOfViewsAsync(long id, CancellationToken token)
        {
            return Session.Query<Document>().Where(w => w.Id == id)
                  .UpdateBuilder()
                  .Set(x => x.Views, x => x.Views + 1)
                  .UpdateAsync(token);
        }

        public Task UpdateNumberOfDownloadsAsync(long id, CancellationToken token)
        {
            return Session.Query<Document>().Where(w => w.Id == id)
                .UpdateBuilder()
                .Set(x => x.Downloads, x => x.Downloads + 1)
                .UpdateAsync(token);
        }


        public override async Task DeleteAsync(Document entity, CancellationToken token)
        {
            
            await 
                Session.CreateSQLQuery("delete from sb.DocumentsTags where documentId = :Id")
                .SetInt64("Id", entity.Id).ExecuteUpdateAsync(token);


            await Session.CreateSQLQuery("delete from sb.[Transaction] where DocumentId = :Id")
                .SetInt64("Id", entity.Id).ExecuteUpdateAsync(token);

            await base.DeleteAsync(entity, token);
        }
    }
}