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
                    new CommandDefinition(@"select sum(b.ItemCount) as itemCount , sum(b.QuizCount) as quizCount , sum(b.FlashcardCount) as flashcardCount
from zbox.box b 
where University in (select id from zbox.University where  needcode = 0)
and Discriminator = 2
and IsDeleted = 0", cancellationToken: token)
                ), token).ConfigureAwait(false);
            return new List<SiteMapCountDto>
            {
                new SiteMapCountDto(SeoType.Item, data.itemCount),
                new SiteMapCountDto(SeoType.Quiz, data.quizCount),
                new SiteMapCountDto(SeoType.Flashcard, data.flashcardCount),
            };
        }

        /*WITH item as (
SELECT COUNT(*) AS itemCount FROM zbox.item i 
WHERE IsDeleted = 0 and i.content is not null

and i.Discriminator = 'FILE'),
quiz AS (
SELECT COUNT(*) as quizCount FROM zbox.Quiz WHERE IsDeleted = 0),
flashcard AS (
SELECT COUNT(*) as flashcardCount FROM zbox.Flashcard WHERE IsDeleted = 0) 
SELECT * FROM item,quiz,flashcard*/
    }
}
