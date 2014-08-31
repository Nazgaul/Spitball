using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class SearchIndex
    {
        public SearchIndex(string name, IEnumerable<SearchField> fields)
        {
            Name = name;
            Fields = fields;
        }

        public string Name { get; set; }
        public IEnumerable<SearchField> Fields { get; set; }
    }

    public class SearchField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Searchable { get; set; }
        public string Filterable { get; set; }
        public string Sortable { get; set; }
        public string Facetable { get; set; }
        public bool Suggestions { get; set; }
        public bool Key { get; set; }
        public string Retrievable { get; set; }
    }

    [DataContract]
    public class SearchUniversity
    {
        [DataMember(Name = "@Search.Action")]
        public string SearchAction { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Extra1 { get; set; }
        [DataMember]
        public string Extra2 { get; set; }
        [DataMember]
        public string Extra3 { get; set; }
        [DataMember]
        public string Extra4 { get; set; }

        [DataMember]
        public string ImageField { get; set; }
    }
}
