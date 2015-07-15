using System.Linq;
using NHibernate.Criterion;
using NHibernate.Linq;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public int GetNumberOfBoxes(University universityId)
        {
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                            .Where(w => w.University == universityId)
                            .And(w => !w.IsDeleted)
                            .RowCount();
        }

        public int GetNumberOfUsers(long universityId)
        {
            return UnitOfWork.CurrentSession.QueryOver<User>()
                .Where(w => w.University.Id == universityId)
                .RowCount();
        }

        public int GetNumberOfQuizzes(long universityId)
        {
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                .Where(w => w.University.Id == universityId)
                .Where(w => w.IsDeleted == false)
                .Select(Projections.Sum<AcademicBox>(s => s.QuizCount))
                .SingleOrDefault<int>();

        }

        public int GetNumberOfItems(long universityId)
        {
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                .Where(w => w.University.Id == universityId)
                .Where(w => w.IsDeleted == false)
                .Select(Projections.Sum<AcademicBox>(s => s.ItemCount))
                .SingleOrDefault<int>();
        }

        public int GetAdminScore(long universityId)
        {
            var sqlQuery = UnitOfWork.CurrentSession.CreateSQLQuery(@"with topreputation as (
                        select userreputation , ROW_NUMBER() OVER(ORDER BY userreputation DESC) AS Row
                         from zbox.users u
                        where universityid = :id
                        )
                        select  top 1 UserReputation  from topreputation 
                        where Row <= (select AdminNoOfPeople from zbox.University where id = :id)
                        order by Row desc");
            sqlQuery.SetInt64("id", universityId);
            
            var retVal = sqlQuery.UniqueResult<int?>();
            if (!retVal.HasValue)
            {
                return int.MaxValue;
            }
            return retVal.Value;
        }


    }
}
