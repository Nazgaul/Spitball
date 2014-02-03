using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
    {
        public University GetUniversity(string name)
        {
            //this cant be done with query over
            var query = UnitOfWork.CurrentSession.CreateQuery("from University where Coalesce(AliasName,Name) = :name");
            query.SetString("name", name.Trim());
            //.QueryOver<University>();
            //query.Where(w => w.AliasName.Coalesce(string.Empty) ==  name.Trim().Lower());
            //query.Where()
            //query.OrderBy()
            //      .SqlFunction("coalesce",
            //                            NHibernateUtil.String,
            //                            Projections.Property<University>(x => x.AliasName),
            //                            Projections.Property<University>(x => x.Name)));
            return query.UniqueResult<University>();
            // return query.SingleOrDefault<University>();
        }
    }
}
