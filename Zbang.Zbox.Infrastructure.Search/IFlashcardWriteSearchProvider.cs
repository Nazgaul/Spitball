using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Qna;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IFlashcardWriteSearchProvider
    {
        Task<bool> UpdateDataAsync(IEnumerable<FlashcardSearchDto> flashcardToUpload, IEnumerable<long> flashcardToDelete, CancellationToken token);
    }

    //public interface IFeedWriteSearchProvider
    //{
    //    Task UpdateDataAsync(FeedSearchDto itemToUpload, IEnumerable<FeedSearchDeleteDto> itemToDelete,
    //        CancellationToken token);
    //}
}