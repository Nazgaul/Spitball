//using Cloudents.Core.DTOs.Admin;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using NHibernate;
//using NHibernate.Linq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Admin
//{
//    public class AllUniversitiesQuery : IQueryAdmin<IList<AllUniversitiesDto>>
//    {
//        public string Country { get; }
//        public AllUniversitiesQuery(string country)
//        {
//            Country = country;
//        }
//        internal sealed class AllUniversitiesEmptyQueryHandler : IQueryHandler<AllUniversitiesQuery, IList<AllUniversitiesDto>>
//        {
//            // private readonly DapperRepository _dapper;
//            private readonly IStatelessSession _session;

//            public AllUniversitiesEmptyQueryHandler(QuerySession session)
//            {
//                _session = session.StatelessSession;
//            }

//            public async Task<IList<AllUniversitiesDto>> GetAsync(AllUniversitiesQuery query, CancellationToken token)
//            {
//                var res = _session.Query<University>()
//                    .Where(w => w.State == ItemState.Ok);
//                if (!string.IsNullOrEmpty(query.Country))
//                {
//                    res = res.Where(w => w.Country == query.Country);
//                }
//                return await res.Select(s => new AllUniversitiesDto
//                {
//                    Id = s.Id,
//                    Name = s.Name
//                }
//                    ).ToListAsync(token);
               
//            }
//        }
//    }
//}
