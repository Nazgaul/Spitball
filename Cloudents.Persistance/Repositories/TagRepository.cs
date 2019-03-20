using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Persistence.Repositories
{
    public class TagRepository : NHibernateRepository<Tag>, ITagRepository
    {
        public TagRepository(ISession session) : base(session)
        {
        }

        public async Task<Tag> GetOrAddAsync(string name, CancellationToken token)
        {
            if (name == null)
            {
                throw new System.ArgumentNullException(nameof(name));
            }
          
            name = name.Trim();

            var tag = await Session.Query<Tag>()
                .Where(w => w.Name.Equals(name))
                .FirstOrDefaultAsync(cancellationToken: token);


            if (tag == null)
            {
                tag = new Tag(name);
                await AddAsync(tag, token);
            }

            return tag;
        }
    }
}