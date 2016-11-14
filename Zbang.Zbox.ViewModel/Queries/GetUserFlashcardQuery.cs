namespace Zbang.Zbox.ViewModel.Queries
{
   public  class GetUserFlashcardQuery
    {
       public GetUserFlashcardQuery(long userId, long flashcardId)
       {
           UserId = userId;
           FlashcardId = flashcardId;
       }

       public long UserId { get; private set; }
       public long FlashcardId { get; private set; }
    }
}
