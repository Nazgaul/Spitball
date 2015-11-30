using System;

namespace Zbang.Zbox.ViewModel.Dto.Search
{
    [Serializable]
    public class SearchItems
    {
      

        //[Obsolete]
       // public string Image { get; set; }


        public string Name { get; set; }
        public long Id { get; set; }
        public string Content { get; set; }

        public string Boxname { get; set; }
        public string UniName { get; set; }
        public string Url { get; set; }

        public string Source { get; set; }

        public long? BoxId { get; set; }
        public string Extension { get; set; }
    }


}
