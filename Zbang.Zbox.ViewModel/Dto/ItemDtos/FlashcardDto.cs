namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class FlashcardDto
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }

        public string Name { get; set; }
        public bool Publish { get; set; }
        public int NumOfViews { get; set; }
    }
}
