using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class SeoFlashcardPage : IReadRepositoryAsync<FlashcardSeoDto, long>
    {
        private readonly DapperRepository _repository;

        public SeoFlashcardPage(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<FlashcardSeoDto> GetAsync(long query, CancellationToken token)
        {
            return _repository.WithConnectionAsync(c => c.QueryFirstOrDefaultAsync<FlashcardSeoDto>(
                @"select u.universityName as UniversityName,u.Country, b.boxId as BoxId,b.boxName as BoxName,f.id,f.name
                from zbox.flashcard f
            join zbox.box b on f.boxId = b.boxId
            left join zbox.university u on u.id = b.university
            where f.id = @FlashcardId
            and f.isDeleted = 0", new {FlashcardId = query}), token);
        }
    }
}
