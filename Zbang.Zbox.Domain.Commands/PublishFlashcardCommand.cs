using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class PublishFlashcardCommand : ICommandAsync, ICommandCache
    {
        public PublishFlashcardCommand(Flashcard flashcard, long boxId)
        {
            Flashcard = flashcard;
            BoxId = boxId;
        }

        public Flashcard Flashcard { get; private set; }
        public long BoxId { get; }
        public CacheRegions CacheRegion => CacheRegions.BuildFeedRegion(BoxId);
    }
}