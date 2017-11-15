using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
quiz AS (
SELECT COUNT(*) as quizCount FROM zbox.Quiz WHERE IsDeleted = 0),
flashcard AS (
SELECT COUNT(*) as flashcardCount FROM zbox.Flashcard WHERE IsDeleted = 0) 
SELECT * FROM item,quiz,flashcard", cancellationToken: token)
                ), token).ConfigureAwait(false);
            var list = new List<SiteMapCountDto>
            {
                new SiteMapCountDto(SeoType.Item, data.itemCount), new SiteMapCountDto(SeoType.Quiz, data.quizCount), new SiteMapCountDto(SeoType.Flashcard, data.flashcardCount),
            };
            return list;
        }
    }
}
