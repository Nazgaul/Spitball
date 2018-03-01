using System.Collections.Generic;
using System.Data;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    //TODO: duplicate
    public class SeoFlashcardRepository : IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>
    {
        private readonly DapperRepository _repository;

        public SeoFlashcardRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<SiteMapSeoDto> Get(SeoQuery query)
        {
            const int pageSize = 49950;
            IEnumerable<SiteMapSeoDto> DoQuery(IDbConnection conn)
            {
                var data = conn.Query<SiteMapSeoDto>(@"with boxSeo as (
 select BoxId, BoxName,u.UniversityName from zbox.box b join zbox.University u on u.id = b.University and needCode = 0
and Discriminator = 2
and b.IsDeleted = 0
)
select b.*,q.Id,q.Name
from zbox.Flashcard q join boxSeo b on q.BoxId = b.BoxId
where q.IsDeleted = 0
and q.publish = 1
order by boxId
offset (@pageNumber)*@rowsPerPage ROWS
FETCH NEXT @rowsPerPage ROWS ONLY", new { rowsPerPage = pageSize, pageNumber = query.Page }, buffered: false);

                foreach (var row in data)
                {
                    yield return row;
                }
            }
            return _repository.WithConnection(DoQuery);
//            using (var conn = _repository.OpenConnection())
//            {
//                var data = conn.Query<SiteMapSeoDto>(@"with boxSeo as (
// select BoxId, BoxName,u.UniversityName from zbox.box b join zbox.University u on u.id = b.University and needCode = 0
//and Discriminator = 2
//and b.IsDeleted = 0
//)
//select b.*,q.Id,q.Name
//from zbox.Flashcard q join boxSeo b on q.BoxId = b.BoxId
//where q.IsDeleted = 0
//and q.publish = 1
//order by boxId
//offset (@pageNumber)*@rowsPerPage ROWS
//FETCH NEXT @rowsPerPage ROWS ONLY", new { rowsPerPage = pageSize, pageNumber = query.Page }, buffered: false);

//                foreach (var row in data)
//                {
//                    //query.Callback(row);
//                    yield return row;
//                }
//            }
        }
    }
}
