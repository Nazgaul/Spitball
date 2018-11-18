using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class TagRepository : NHibernateRepository<Tag>, ITagRepository
    {
        public TagRepository(ISession session) : base(session)
        {
        }

        public async Task<Tag> GetOrAddAsync(string name, CancellationToken token)
        {
            var tag = await GetAsync(name, token);

            if (tag == null)
            {

                tag = new Tag(name);
                await AddAsync(tag, token).ConfigureAwait(true);
            }

            return tag;
        }
    }
}