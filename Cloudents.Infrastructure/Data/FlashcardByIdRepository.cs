using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Entities.DocumentDb;
using Cloudents.Application.Interfaces;

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
