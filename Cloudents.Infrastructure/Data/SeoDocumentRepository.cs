using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class SeoDocumentRepository : IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>
    {
        private readonly DapperRepository _repository;

        public SeoDocumentRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<SiteMapSeoDto> Get(SeoQuery query)
        {
            const int pageSize = 49950;
            using (var conn = _repository.OpenConnection())
            {
                var data = conn.Query<SiteMapSeoDto>(@"WITH boxSeo as (
 select BoxId, BoxName,u.UniversityName from zbox.box b LEFT JOIN zbox.University u on u.id = b.University
and Discriminator = 2
and b.IsDeleted = 0
)
select b.*,i.ItemId as id,i.Name
from zbox.item i join boxSeo b on i.BoxId = b.BoxId
where i.IsDeleted = 0
and i.content is not null
and i.Discriminator = 'FILE'
order by boxId
offset (@pageNumber)*@rowsPerPage ROWS
FETCH NEXT @rowsPerPage ROWS ONLY", new { rowsPerPage = pageSize, pageNumber = query.Page }, buffered: false);

                foreach (var row in data)
                {
                    yield return row;
                }
            }
        }
    }
}
