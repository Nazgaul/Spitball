using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class SeoDocumentRepository : IReadRepositoryAsync<IEnumerable<SiteMapSeoDto>, SeoQuery>
    {
        private readonly DapperRepository _repository;

        public SeoDocumentRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public  Task<IEnumerable<SiteMapSeoDto>> GetAsync(SeoQuery query, CancellationToken token)
        {
            const int pageSize = 49950;
            return Task.FromResult<IEnumerable<SiteMapSeoDto>>(null);
//            var result = await _repository.WithConnectionAsync(async c =>
//            {
//                var data = await c.QueryAsync<SiteMapSeoDto>(new CommandDefinition(@"WITH boxSeo as (
// select BoxId, BoxName,u.UniversityName from zbox.box b LEFT JOIN zbox.University u on u.id = b.University
//and Discriminator = 2
//and b.IsDeleted = 0
//)
//select b.*,i.ItemId as id,i.Name
//from zbox.item i join boxSeo b on i.BoxId = b.BoxId
//where i.IsDeleted = 0
//and i.content is not null
//and i.Discriminator = 'FILE'
//order by boxId
//offset (@pageNumber)*@rowsPerPage ROWS
//FETCH NEXT @rowsPerPage ROWS ONLY", new { rowsPerPage = pageSize, pageNumber = query },cancellationToken: token, flags: CommandFlags.None)
//                ).ConfigureAwait(false);

//                foreach (var row in data)
//                {
//                    query.Callback(row);
//                    //   // yield return row;
//                }

//            }, token).ConfigureAwait(false);
//            return result;
        }

        //IDbConnection, Task<T>
//        private async Task<IEnumerable<SiteMapSeoDto>> GetDataAsync(IDbConnection c)
//        {
//            IEnumerable<SiteMapSeoDto> data = await c.QueryAsync<SiteMapSeoDto>(new CommandDefinition(@"WITH boxSeo as (
// select BoxId, BoxName,u.UniversityName from zbox.box b LEFT JOIN zbox.University u on u.id = b.University
//and Discriminator = 2
//and b.IsDeleted = 0
//)
//select b.*,i.ItemId as id,i.Name
//from zbox.item i join boxSeo b on i.BoxId = b.BoxId
//where i.IsDeleted = 0
//and i.content is not null
//and i.Discriminator = 'FILE'
//order by boxId
//offset (@pageNumber)*@rowsPerPage ROWS
//FETCH NEXT @rowsPerPage ROWS ONLY", new { rowsPerPage = 50, pageNumber = 0 }, flags: CommandFlags.None)
//            ).ConfigureAwait(false);

//            foreach (var row in data)
//            {
//                yield return row;
//            }
//        }
    }
}
