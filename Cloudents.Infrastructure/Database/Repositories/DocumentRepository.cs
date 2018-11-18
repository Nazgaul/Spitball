﻿using Cloudents.Core.Entities.Db;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class DocumentRepository : NHibernateRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(ISession session) : base(session)
        {


        }
        public Task UpdateNumberOfViews(long id, CancellationToken token)
        {
            return Session.Query<Document>().Where(w => w.Id == id)
                  .UpdateBuilder()
                  .Set(x => x.Views, x => x.Views + 1)
                  .UpdateAsync(token);

        }
    }
}