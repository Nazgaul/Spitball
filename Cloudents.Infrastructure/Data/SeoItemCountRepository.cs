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
    public class SeoItemCountRepository : IReadRepositorySingle<IEnumerable<SiteMapDto>>
    {
        private readonly DapperRepository _repository;

        public SeoItemCountRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SiteMapDto>> GetAsync(CancellationToken token)
        {
            var data = await _repository.WithConnectionAsync(c =>
                c.QueryFirstOrDefaultAsync(
                    new CommandDefinition(@"WITH item as (
SELECT COUNT(*) AS itemCount FROM zbox.item WHERE IsDeleted = 0),
quiz AS (
SELECT COUNT(*) as quizCount FROM zbox.Quiz WHERE IsDeleted = 0),
flashcard AS (
SELECT COUNT(*) as flashcardCount FROM zbox.Flashcard WHERE IsDeleted = 0) 
SELECT * FROM item,quiz,flashcard", cancellationToken: token)
                ), token).ConfigureAwait(false);
            var list = new List<SiteMapDto>
            {
                new SiteMapDto(SeoType.Item, data.itemCount), new SiteMapDto(SeoType.Quiz, data.quizCount), new SiteMapDto(SeoType.Flashcard, data.flashcardCount),
            };
            return list;
        }
    }
}
