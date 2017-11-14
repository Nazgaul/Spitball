using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class SeoDocumentPage : IReadRepositoryAsync<DocumentSeoDto, long>
    {
        private readonly DapperRepository _repository;

        public SeoDocumentPage(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<DocumentSeoDto> GetAsync(long query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryFirstOrDefaultAsync<DocumentSeoDto>(
                @"select i.name
,i.Content as Description
,u.Country
,b.BoxName
,i.Discriminator
,i.blobName as ImageUrl
,l.name as departmentName
from zbox.item i 
join zbox.box b on i.BoxId = b.BoxId
left join zbox.library l on b.libraryId = l.libraryId
where itemId = @ItemId
and i.IsDeleted = 0;", new { ItemId = query }), token);
        }
    }
}
