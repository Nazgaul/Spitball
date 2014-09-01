using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class AcademicBoxRepository : NHibernateRepository<AcademicBox>, IAcademicBoxRepository
    {
        public AcademicBox CheckIfExists(string courseCode, Department department, string professor, string courseName)
        {
            if (!string.IsNullOrWhiteSpace(professor))
            {
                professor = professor.Trim().ToLower();
            }
            if (!string.IsNullOrWhiteSpace(courseCode))
            {
                courseCode = courseCode.Trim().ToLower();
            }
            var query = UnitOfWork.CurrentSession.QueryOver<AcademicBox>();
            query.Where(b => b.Department == department);
            query.Where(b => b.IsDeleted == false);
            query.Where(b => b.CourseCode.TrimStr().Lower() == courseCode);
            query.Where(b => b.Professor.TrimStr().Lower() == professor);
            query.Where(b => b.Name == courseName.Trim());
            return query.SingleOrDefault<AcademicBox>();

        }

        //public AcademicBox GetByNameAndProfessor(string boxName, Professor professor, User universityUser)
        //{
        //    var query = UnitOfWork.CurrentSession.QueryOver<AcademicBox>();
        //    query.Where(b => b.Owner == universityUser);
        //    query.Where(b => b.IsDeleted == false);
        //    query.Where(b => b.Name == boxName.Trim().Lower());
        //    query.Where(b => b.Professor == professor);
        //    return query.SingleOrDefault<AcademicBox>();
        //}
    }
}
