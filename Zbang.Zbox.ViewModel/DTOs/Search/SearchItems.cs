namespace Zbang.Zbox.ViewModel.DTOs.Search
{
    public class SearchItems
    {
        public SearchItems(string image, string name, long id, string type, string content, double rate, int views, string boxName, long boxid, string universityName, string uniName)
        {
            Image = image;// Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(image);
            Name = name;
            Id = id;
            Boxname = boxName;
            Boxid = boxid;
            Universityname = universityName;
            Content = content;
            Rate = rate;
            Views = views;
            UniName = uniName;
        }
        public string Image { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Boxname { get; set; }
        public long Boxid { get; set; }
        public string Content { get; set; }
        public double Rate { get; set; }
        public int Views { get; set; }

        public string Universityname { get; set; }

        public string Url { get; set; }
        public string UniName { get; set; }
    }
}
