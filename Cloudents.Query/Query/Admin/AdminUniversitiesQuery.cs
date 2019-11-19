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
    public class AdminUniversitiesQuery : IQueryAdmin<IList<PendingUniversitiesDto>>
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

        private readonly IStatelessSession _session;


        public AdminPendingUniversitiesQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<PendingUniversitiesDto>> GetAsync(AdminUniversitiesQuery query, CancellationToken token)
        {
            var q = _session.Query<University>()
                .Where(w => w.State == query.State);
            if (!string.IsNullOrEmpty(query.Country))
            {
                q = q.Where(w => w.Country == query.Country);
            }
            return await q.Select(s => new PendingUniversitiesDto
            {
                Id = s.Id,
                Name = s.Name,
                Created = s.RowDetail.CreationTime,
                CanBeDeleted = !_session.Query<Document>().Any(a => a.University.Id == s.Id && a.University.Id != null) &&
                                !_session.Query<Question>().Any(a => a.University.Id == s.Id && a.University.Id != null)
            }).ToListAsync(token);
            //var sql = $@"select Id, Name, CreationTime as Created from sb.University where State = @state";

            //if (!string.IsNullOrEmpty(query.Country))
            //{
            //    sql += $" and country = @country";
            //    //if (query.Country.Equals("il", StringComparison.InvariantCultureIgnoreCase))
            //    //{
            //    //    sql += " and country = 'il'";
            //    //}
            //    //else if (query.Country.Equals("us", StringComparison.InvariantCultureIgnoreCase))
            //    //{
            //    //    sql += " and country = 'us'";
            //    //}

            //}
            //using (var connection = _dapper.OpenConnection())
            //{
            //    var res = await connection.QueryAsync<PendingUniversitiesDto>(sql, new { state = query.State.ToString() });
            //    return res.AsList();
            //}
        }
    }
}
