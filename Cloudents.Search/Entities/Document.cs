using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Common.Enum;
using Cloudents.Core.DTOs.SearchSync;
using Cloudents.Core.Enum;
using Cloudents.Search.Interfaces;

namespace Cloudents.Search.Entities
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "json.net need public set")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "json.net need public set")]

    public class Document : ISearchObject
    {

        public static Document FromDto(DocumentSearchDto obj)
        {
            return new Document
            {
                University = obj.University.ToString(),
                DateTime = obj.DateTime,
                Country = obj.Country.ToUpperInvariant(),
                Course = obj.Course.ToUpperInvariant(),
                Id = obj.ItemId.ToString(),
                Name = obj.Name,
               // Language = obj.Language.ToLowerInvariant(),
                Type = obj.Type,
                Tags = obj.TagsArray
            };
        }

       
       

        public string Id { get; set; }

        public string Name { get; set; }
        public string MetaContent { get; set; }
        public string Content { get; set; }

        public string[] Tags { get; set; }
        public string Course { get; set; }
        public string Country { get;  set; }
        //public string Language { get;  set; }
        public string University { get;  set; }


        public DateTimeOffset? DateTime { get; set; }

        public DocumentType Type { get; set; }

    }
}