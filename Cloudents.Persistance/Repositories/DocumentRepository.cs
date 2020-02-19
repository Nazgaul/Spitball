using Cloudents.Core.Entities;
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
        public Task UpdateNumberOfViews(long id, CancellationToken token)
        {
            return Session.Query<Document>().Where(w => w.Id == id)
                  .UpdateBuilder()
                  .Set(x => x.Views, x => x.Views + 1)
                  .UpdateAsync(token);
        }

        public Task UpdateNumberOfDownloads(long id, CancellationToken token)
        {
            return Session.Query<Document>().Where(w => w.Id == id)
                .UpdateBuilder()
                .Set(x => x.Downloads, x => x.Downloads + 1)
                .UpdateAsync(token);
        }

    }
}