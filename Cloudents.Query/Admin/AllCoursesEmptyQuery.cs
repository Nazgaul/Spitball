//using Cloudents.Core.Entities;
//using NHibernate;
//using NHibernate.Linq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Admin
//{
//    public class AllCoursesEmptyQuery : IQuery<IList<string>>
//    {
//        internal sealed class AllCoursesEmptyQueryHandler : IQueryHandler<AllCoursesEmptyQuery, IList<string>>
//        {
//            private readonly IStatelessSession _session;

//            public AllCoursesEmptyQueryHandler(QuerySession session)
//            {
//                _session = session.StatelessSession;
//            }

//            public async Task<IList<string>> GetAsync(AllCoursesEmptyQuery query, CancellationToken token)
//            {
//                return await _session.Query<Course>()
//                    .Where(w => w.State == Core.Enum.ItemState.Ok)
//                    .Select(s => s.Id).ToListAsync(token);
//                //const string sql = @"select Name from sb.Course where State = 'Ok'";
//                //using (var connection = _dapper.OpenConnection())
//                //{
//                //    var res = await connection.QueryAsync<string>(sql);
//                //    return res.AsList();
//                //}
//            }
//        }
//    }
//}
