using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Search
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]

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


        public DateTimeOffset? DateTime { get; set; }

        public DocumentType Type { get; set; }

    }
}