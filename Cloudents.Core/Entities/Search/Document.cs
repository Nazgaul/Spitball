﻿using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    public class Document : ISearchObject
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string MetaContent { get; set; }
        public string Content { get; set; }

        public string[] Tags { get; set; }
        public string Course { get; set; }
        public string Country { get;  set; }
        public string Language { get;  set; }
        public string University { get;  set; }
        public DateTime DateTime { get; set; }

    }
}