namespace Zbang.Zbox.ViewModel.Dto.Search
{
    public class SearchFlashcard
    {
        public SearchFlashcard()
        {

        }
        public SearchFlashcard(string name, long id, string content,
            string boxName, string uniName, long boxId)
        {
            Name = name;
            Id = id;
            Content = content;
            UniName = uniName;
            BoxName = boxName;
            BoxId = boxId;
        }
        public string Name { get; set; }

        public long Id { get; set; }
        public string Content { get; set; }

        public string BoxName { get; set; }
        public string UniName { get; set; }

        public long BoxId { get; set; }
    }
}
