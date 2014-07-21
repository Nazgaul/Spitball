
using System.Linq;
namespace Zbang.Zbox.Infrastructure.Repositories
{
    public interface IRepository<TEntity>
    {
        TEntity Get(object id);
        TEntity Load(object id);
        void Save(TEntity item, bool flush = false);
        void Save(TEntity item, object id);
        void Delete(TEntity item);

        IQueryable<TEntity> GetQuerable();
        
    }
}
