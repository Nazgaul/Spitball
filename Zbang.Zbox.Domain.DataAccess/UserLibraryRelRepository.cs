using System;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UserLibraryRelRepository : NHibernateRepository<UserLibraryRel>, IUserLibraryRelRepository 
    {
        public UserLibraryRel GetUserLibraryRelationship(long userId, Guid departmentId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserLibraryRel>().
                Where(w => w.User.Id == userId)
                .Where(w => w.Library.Id == departmentId).SingleOrDefault();
        }
    }


    public class LibraryRepository : NHibernateRepository<Library>, ILibraryRepository
    {
        public Guid GetTopTreeNode(Guid departmentId)
        {
            var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(@"with node as (
select level,Id from zbox.library where Libraryid = :parentid
)

SELECT top 1
    l.libraryid
FROM zbox.Library l , node n
where (n.level).IsDescendantOf(l.level) = 1 and l.id = n.Id");
            sqlQuery.SetGuid("parentid", departmentId);
            sqlQuery.SetReadOnly(true);
            return sqlQuery.UniqueResult<Guid>();
        }
    }
}