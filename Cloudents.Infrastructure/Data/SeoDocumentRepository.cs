using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Dapper;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [UsedImplicitly]
    public class SeoDocumentRepository : IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>
    {
        private readonly DapperRepository _repository;
        private readonly SeoDbQuery _sqlQuery;

        public SeoDocumentRepository(DapperRepository repository, SeoDbQuery query)
        {
            _repository = repository;
            _sqlQuery = query;
        }

        public IEnumerable<SiteMapSeoDto> Get(SeoQuery query)
        {
            const int pageSize = 49950;

            using (var conn = _repository.OpenConnection())
            {
                var data = conn.Query<SiteMapSeoDto>(
                   new CommandDefinition(
                       _sqlQuery.Query,
                       new { rowsPerPage = pageSize, pageNumber = query.Page },
                       flags: CommandFlags.None,
                       commandTimeout: 90
                       )

                   );

                foreach (var row in data)
                {
                    yield return row;
                }
            }
            //_repository.WithConnection()
            //IEnumerable<SiteMapSeoDto> DoQuery(IDbConnection conn)
            //{
            //    var data = conn.Query<SiteMapSeoDto>(_sqlQuery.Query, new { rowsPerPage = pageSize, pageNumber = query.Page }, buffered: false);

            //    foreach (var row in data)
            //    {
            //        yield return row;
            //    }
            //}
            //return _repository.WithConnection(DoQuery);
        }

    }
}
