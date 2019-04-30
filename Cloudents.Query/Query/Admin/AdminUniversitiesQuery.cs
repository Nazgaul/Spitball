using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Enum;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminUniversitiesQuery: IQuery<IList<PendingUniversitiesDto>>
    {
        public AdminUniversitiesQuery(string country, ItemState state)
        {
            Country = country;
            State = state;
        }
        public string Country { get; }
        public ItemState State { get; }
    }

    internal class AdminPendingUniversitiesQueryHandler : IQueryHandler<AdminUniversitiesQuery, IList<PendingUniversitiesDto>>
    {

        private readonly DapperRepository _dapper;


        public AdminPendingUniversitiesQueryHandler(DapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IList<PendingUniversitiesDto>> GetAsync(AdminUniversitiesQuery query, CancellationToken token)
        {
            var sql = $@"select Id, Name, CreationTime as Created from sb.University where State = @state";

            if (!string.IsNullOrEmpty(query.Country))
            {
                if (query.Country.Equals("il", StringComparison.InvariantCultureIgnoreCase))
                {
                    sql += " and country = 'il'";
                }
                else if (query.Country.Equals("us", StringComparison.InvariantCultureIgnoreCase))
                {
                    sql += " and country = 'us'";
                }
            }
            using (var connection = _dapper.OpenConnection())
            {
                var res = await connection.QueryAsync<PendingUniversitiesDto>(sql, new { state = query.State.ToString() });
                return res.AsList();
            }
        }
    }
}
