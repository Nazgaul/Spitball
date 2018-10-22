﻿using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    public class University : ISearchObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public string Extra { get; set; }
        public string[] Prefix { get; set; }

        public string Country { get; set; }
    }
}