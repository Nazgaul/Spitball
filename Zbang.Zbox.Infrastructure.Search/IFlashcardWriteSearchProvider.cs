using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IFlashcardWriteSearchProvider
    {
        Task<bool> UpdateDataAsync(IEnumerable<FlashcardSearchDto> flashcardToUpload, IEnumerable<long> flashcardToDelete, CancellationToken token);
    }
}