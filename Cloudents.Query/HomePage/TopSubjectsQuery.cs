//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using NHibernate;
//using NHibernate.Linq;

//namespace Cloudents.Query.HomePage
//{
//    public class TopSubjectsQuery : IQuery<IEnumerable<string>>
//    {
//        public TopSubjectsQuery(string language)
//        {
//            UserLanguage = language;
//        }
//        public string UserLanguage { get; set; }
//        internal sealed class TopSubjectsQueryHandler : IQueryHandler<TopSubjectsQuery, IEnumerable<string>>
//        {
//            private readonly IStatelessSession _session;

//            public TopSubjectsQueryHandler(QuerySession session)
//            {
//                _session = session.StatelessSession;
//            }

//            public async Task<IEnumerable<string>> GetAsync(TopSubjectsQuery query, CancellationToken token)
//            {
//                var res = _session.Query<CourseSubject>();
            
//                return await res.Select(s => !string.IsNullOrEmpty(query.UserLanguage) && query.UserLanguage.Equals(Language.Hebrew.Info.Name) ? s.Name : s.EnglishName).Take(5).ToListAsync(token);
             
//            }
//        }
//    }
//}
