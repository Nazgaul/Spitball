﻿using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Search
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
}
