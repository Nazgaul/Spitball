using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class SeoItemCountRepository : IReadRepositoryAsync<IEnumerable<SiteMapCountDto>>
    {
        private readonly DapperRepository _repository;

        public SeoItemCountRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SiteMapCountDto>> GetAsync(CancellationToken token)
        {
            var data = await _repository.WithConnectionAsync(c =>
                c.QueryFirstOrDefaultAsync(
                    new CommandDefinition(@"WITH item as (
SELECT COUNT(*) AS itemCount FROM zbox.item i WHERE IsDeleted = 0 and i.content is not null
and i.Discriminator = 'FILE'),
flashcard AS (
SELECT COUNT(*) as flashcardCount FROM zbox.Flashcard WHERE IsDeleted = 0) 
SELECT * FROM item,flashcard", cancellationToken: token)
                ), token).ConfigureAwait(false);
            return new List<SiteMapCountDto>
            {
                new SiteMapCountDto(SeoType.Item, data.itemCount),
                new SiteMapCountDto(SeoType.Flashcard, data.flashcardCount),
            };
        }
    }
}
