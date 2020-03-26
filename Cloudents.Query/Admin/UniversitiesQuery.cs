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
//    public class UniversitiesQuery : IQueryAdmin<IList<PendingUniversitiesDto>>
//    {
//        public UniversitiesQuery(string country, ItemState state)
//        {
//            Country = country;
//            State = state;
//        }
//        public string Country { get; }
//        public ItemState State { get; }
//    }

//    internal sealed class PendingUniversitiesQueryHandler : IQueryHandler<UniversitiesQuery, IList<PendingUniversitiesDto>>
//    {

//        private readonly IStatelessSession _session;


//        public PendingUniversitiesQueryHandler(QuerySession session)
//        {
//            _session = session.StatelessSession;
//        }

//        public async Task<IList<PendingUniversitiesDto>> GetAsync(UniversitiesQuery query, CancellationToken token)
//        {
//            var q = _session.Query<University>()
//                .Where(w => w.State == query.State);
//            if (!string.IsNullOrEmpty(query.Country))
//            {
//                q = q.Where(w => w.Country == query.Country);
//            }
//            return await q.Select(s => new PendingUniversitiesDto
//            {
//                Id = s.Id,
//                Name = s.Name,
//                Created = s.RowDetail.CreationTime,
//                CanBeDeleted = !_session.Query<Document>().Any(a => a.University.Id == s.Id && a.University.Id != null) &&
//                                !_session.Query<Question>().Any(a => a.University.Id == s.Id && a.University.Id != null)
//            }).ToListAsync(token);
          
//        }
//    }
//}
