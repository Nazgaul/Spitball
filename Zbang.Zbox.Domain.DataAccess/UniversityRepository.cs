using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UniversityStats
    {
        public int UsersCount { get; set; }
        public int BoxesCount { get; set; }
        public int ItemsCount { get; set; }
        public int QuizzesCount { get; set; }


    }
    public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public int GetNumberOfBoxes(University universityId)
        {
            return UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                            .Where(w => w.University == universityId)
                            .And(w => !w.IsDeleted)
                            .RowCount();
        }

        

       

        public UniversityStats GetStats(long universityId)
        {

            var usersFuture = UnitOfWork.CurrentSession.QueryOver<User>()
                 .Where(w => w.University.Id == universityId)
                 .Select(Projections.RowCount())
                 .FutureValue<int>();

            

            var countFuture = UnitOfWork.CurrentSession.QueryOver<AcademicBox>()
                .Where(w => w.University.Id == universityId)
                .Where(w => w.IsDeleted == false)
                .SelectList(list => list
                    .SelectSum(s => s.QuizCount)
                    .SelectSum(s => s.ItemCount)
                    .SelectSum(s=> s.FlashcardCount)
                    .Select(Projections.RowCount())
                ).FutureValue<object[]>();

            var usersCount = usersFuture.Value;
            var boxStats = countFuture.Value;
            return new UniversityStats
            {
                BoxesCount = (int?) boxStats[2] ?? 0,
                ItemsCount = (int?) boxStats[1] ?? 0,
                QuizzesCount = (int?) boxStats[0] ?? 0,
                UsersCount = usersCount
            };

        }


        public int GetAdminScore(long universityId)
        {
            var numberofAdminsQuery = UnitOfWork.CurrentSession.CreateSQLQuery("select AdminNoOfPeople from zbox.University where id = :id");
            numberofAdminsQuery.SetInt64("id", universityId);
            var adminValue = numberofAdminsQuery.UniqueResult<int>();

            if (adminValue == 0)
            {
                return int.MaxValue;
            }

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
                return 0;
            }
            return retVal.Value;
        }


    }
}
