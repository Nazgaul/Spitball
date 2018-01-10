using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Data
{
    public class EfRepository<T> : IRepository<T> where T:class
    {
        private readonly AppDbContext _dbContext;

        public EfRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity, CancellationToken token)
        {
            await _dbContext.Set<T>().AddAsync(entity, token).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync(token).ConfigureAwait(false);
            return entity;
        }
    }
}
