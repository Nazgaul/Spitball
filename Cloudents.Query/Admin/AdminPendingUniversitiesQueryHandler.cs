//using Cloudents.Core.DTOs.Admin;
//using Cloudents.Query.Query.Admin;
//using Dapper;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Admin
//{
//    public class AdminPendingUniversitiesQueryHandler : IQueryHandler<AdminLanguageQuery, IList<PendingUniversitiesDto>>
//    {
//        private readonly DapperRepository _dapper;


//        public AdminPendingUniversitiesQueryHandler(DapperRepository dapper)
//    {
//            _dapper = dapper;
//    }

//    public async Task<IList<PendingUniversitiesDto>> GetAsync(AdminLanguageQuery query, CancellationToken token)
//    {
//        var sql = @"select Id, Name, CreationTime as Created from sb.University where State = 'Pending'";
//            if (query.Language == "il")
//            {
//                sql += "and country = 'il'";
//            }
//            else if (query.Language == "us")
//            {
//                sql += "and country = 'us'";
//            }
//            return await _dapper.WithConnectionAsync(async connection =>
//            {
//                var res = await connection.QueryAsync<PendingUniversitiesDto>(sql);
//                return res.AsList();
//            }, token);
//    }
//}
//}
