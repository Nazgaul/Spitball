namespace Zbang.Zbox.ViewModel.Dto.Search
{
    public class SearchItems
    {
        public SearchItems()
        {
            
        }
        public SearchItems(string image, string name, long id,  string content,
            string url)
        {
            Image = image;
            Name = name;
            Id = id;
            Content = content;
            Url = url;
        }
        public string Image { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Content { get; set; }


        public string Url { get; set; }
    }
}
