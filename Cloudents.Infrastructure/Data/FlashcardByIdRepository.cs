using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.DocumentDb;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Data
{
    public class FlashcardByIdRepository : IReadRepositoryAsync<Flashcard, long>
    {
        private readonly IDocumentDbRepository<Flashcard> _repository;

        public FlashcardByIdRepository(IDocumentDbRepository<Flashcard> repository)
        {
            _repository = repository;
        }

        public Task<Flashcard> GetAsync(long query, CancellationToken token)
        {
            return _repository.GetItemAsync(query.ToString());
        }
    }
}
