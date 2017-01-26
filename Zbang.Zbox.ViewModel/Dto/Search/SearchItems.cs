using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Search
{
    [Serializable]
    public class SearchDocument
    {
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

    public abstract class SearchJaredDto
    {
        
    }

    public class SearchItemResult : SearchJaredDto
    {
        public IEnumerable<SearchItem> Result { get; set; }
        public Dictionary<string, IEnumerable<FacetResult>> Facet { get; set; }

    }

    public class FacetResult
    {
        public string Name { get; set; }
        public long Value { get; set; }
    }

    public class SearchItem 
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Content { get; set; }

        public string ContentEn { get; set; }

        public string ContentHe { get; set; }

        public string University { get; set; }

        public string Course { get; set; }

        public string Professor { get; set; }

        public string Code { get; set; }

        public int Type { get; set; }

        public string[] Tags { get; set; }
    }


}
