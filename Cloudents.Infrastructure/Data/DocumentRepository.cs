using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class DocumentRepository : IReadRepositoryAsync<DocumentDto, long>
    {
        private readonly DapperRepository _repository;

        public DocumentRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<DocumentDto> GetAsync(long query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryFirstOrDefaultAsync<DocumentDto>(
                @"select 
 i.Name as name,
 i.creationTime as date,
    u.UserName as owner, 
    i.BlobName as blob,
    i.Discriminator as type
    from zbox.Item i
    join zbox.Users u on u.UserId = i.UserId
    where i.ItemId = @ItemId
    and i.IsDeleted = 0 ;", new { ItemId = query }), token);
        }
    }
}
