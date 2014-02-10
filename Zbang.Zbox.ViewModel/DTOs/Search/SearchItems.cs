using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Search
{
    public class SearchItems
    {
        public SearchItems(string image, string name, long id, string type, string boxName, long boxid, string universityName)
        {
            if (type == "File")
            {
                Image = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailUrl(image);
            }
            else
            {
                Image = Zbang.Zbox.Infrastructure.Storage.BlobProvider.GetThumbnailLinkUrl();
            }
            Name = name;
            Id = id;
            Boxname = boxName;
            Boxid = boxid;
            Universityname = universityName;
        }
        public string Image {get;set;}
        public string Name { get; set; }
        public long Id { get; set; }
        public string Boxname { get; set; }
        public long Boxid { get; set; }
        public string Universityname { get; set; }

        public string Url { get; set; }
    }
}
