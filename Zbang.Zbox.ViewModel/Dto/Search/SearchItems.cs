using System;

namespace Zbang.Zbox.ViewModel.Dto.Search
{
    [Serializable]
    public class SearchItems
    {
        public SearchItems()
        {
            
        }
        public SearchItems(string image, string name, long id,  string content,
            string boxName, string uniName,string url)
        {
            Image = image;
            Name = name;
            Id = id;
            Content = content;
            UniName = uniName;
            Boxname = boxName;
            Url = url;
        }
        public string Image { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public string Content { get; set; }

        public string Boxname { get; set; }
        public string UniName { get; set; }
        public string Url { get; set; }
    }
}
