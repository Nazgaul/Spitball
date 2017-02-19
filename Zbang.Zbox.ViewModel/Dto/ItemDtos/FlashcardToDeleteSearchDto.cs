namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FlashcardToDeleteSearchDto : ItemToDeleteSearchDto
    {
        public override string SearchContentId => "flashcard_" + Id;
    }
}