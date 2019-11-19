using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminAllUniversitiesQuery : IQueryAdmin<IList<AllUniversitiesDto>>
    {
        public string Country { get; }
        public AdminAllUniversitiesQuery(string country)
        {
            Country = country;
        }
        internal sealed class AdminAllUniversitiesEmptyQueryHandler : IQueryHandler<AdminAllUniversitiesQuery, IList<AllUniversitiesDto>>
        {
            // private readonly DapperRepository _dapper;
            private readonly IStatelessSession _session;

            public AdminAllUniversitiesEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<AllUniversitiesDto>> GetAsync(AdminAllUniversitiesQuery query, CancellationToken token)
            {
                var res = _session.Query<University>()
                    .Where(w => w.State == ItemState.Ok);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    res = res.Where(w => w.Country == query.Country);
                }
                return await res.Select(s => new AllUniversitiesDto
                {
                    Id = s.Id,
                    Name = s.Name
                }
                    ).ToListAsync(token);
                //const string sql = @"select Id,Name from sb.University where  and State = 'Ok'";
                //using (var connection = _dapper.OpenConnection())
                //{
                //    var res = await connection.QueryAsync<AllUniversitiesDto>(sql);
                //    return res.AsList();
                //}
            }
        }
    }
}
