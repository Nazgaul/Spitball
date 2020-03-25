//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Interfaces;
//using NHibernate;
//using NHibernate.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Persistence.Repositories
//{
//    public class UniversityRepository : NHibernateRepository<University>, IUniversityRepository
//    {
//        public UniversityRepository(ISession session) : base(session)
//        {
//        }

//        public async Task<University> GetUniversityByNameAndCountryAsync(string name, string country, CancellationToken token)
//        {
//            return await Session.Query<University>()
//                .FirstOrDefaultAsync(w => w.Name.Equals(name.Trim()) && w.State == ItemState.Ok && w.Country == country.Trim(), 
//                token);
//        }
//    }
//}
