//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using NHibernate;
//using NHibernate.Linq;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Universities
//{
//    public class UniversityQuery : IQuery<UniversityDto>
//    {
//        public UniversityQuery(Guid universityId)
//        {
//            UniversityId = universityId;
//        }

//        private Guid UniversityId { get; }



//        internal sealed class UniversityQueryHandler : IQueryHandler<UniversityQuery, UniversityDto>
//        {
//            private readonly IStatelessSession _session;

//            public UniversityQueryHandler(QuerySession session)
//            {
//                _session = session.StatelessSession;
//            }

//            public Task<UniversityDto> GetAsync(UniversityQuery query, CancellationToken token)
//            {

//                return _session.Query<University>()
//                    .Where(w => w.Id == query.UniversityId)
//                    .Select(s => new UniversityDto(s.Id, s.Name, s.Country, s.Image, s.UsersCount))
//                    .SingleOrDefaultAsync(token);

//            }

//        }
//    }
//}
