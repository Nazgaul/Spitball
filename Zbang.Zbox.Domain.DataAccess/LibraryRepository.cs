using System;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class LibraryRepository : NHibernateRepository<Library>, ILibraryRepository
    {
        public Guid GetTopTreeNode(Guid departmentId)
        {
            var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(@"with name_tree as (
   select libraryid, parentId, name
   from zbox.library
   where libraryid = :parentid -- this is the starting point you want in your recursion
   union all
   select c.libraryid, c.parentId, c.name
   from zbox.library c
     join name_tree p on p.parentId = c.libraryid  -- this is the recursion
) 
select libraryid
from name_tree
where  parentId is null");
            sqlQuery.SetGuid("parentid", departmentId);
            sqlQuery.SetReadOnly(true);
            return sqlQuery.UniqueResult<Guid>();
        }

        public void UpdateElementToIsDirty(Guid departmentId)
        {
            var queryItem = UnitOfWork.CurrentSession.GetNamedQuery("updateItem");
            queryItem.SetGuid("depId", departmentId);
            queryItem.ExecuteUpdate();

            var queryQuiz = UnitOfWork.CurrentSession.GetNamedQuery("updateQuiz");
            queryQuiz.SetGuid("depId", departmentId);
            queryQuiz.ExecuteUpdate();

            var queryBox = UnitOfWork.CurrentSession.GetNamedQuery("updateBox");
            queryBox.SetGuid("depId", departmentId);
            queryBox.ExecuteUpdate();
        }
    }
}