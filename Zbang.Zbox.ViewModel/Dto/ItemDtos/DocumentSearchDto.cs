using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class DocumentSearchDto
    {
        public DocumentSearchDto()
        {
            Tags = new List<ItemSearchTag>();
        }
        public long Id { get; set; }

        public ItemUniversitySearchDto University { get; set; }
        public ItemCourseSearchDto Course { get; set; }

        public Language? Language { get; set; }

        public List<ItemSearchTag> Tags { get; set; }

        public DateTime Date { get; set; }

        public string BlobName { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }



       // private IEnumerable<ItemType> m_Type;
        public string Image { get; set; }

        // public override string Content { get; set; }
        public string DocumentContent { get; set; }

        public string Name => Path.GetFileNameWithoutExtension(FileName);
        public string Content => DocumentContent;

        public bool PreviewFailed { get; set; }

        public string FileName { get; set; }
        public ItemType Type
        {
            get
            {
                return TypeDocument.ToLowerInvariant() == "file" ? ItemType.Document : ItemType.Link;
                //if (m_Type != null && m_Type.Any())
                //{
                //    return m_Type;
                //}
                //var type = new List<ItemType>
                //{
                //    TypeDocument.ToLowerInvariant() == "file" ? ItemType.Document : ItemType.Link
                //};

                //if (DocType.Equals(ItemType.Undefined) && string.IsNullOrEmpty(Content))
                //{
                //    return type;
                //}
                //type.Add(DocType);
                //m_Type = type;
                //return m_Type;
            }

        }

        // public string SearchContentId => "item_" + Id;

        public string Url { get; set; }

        //public ItemType DocType { get; set; }

        public string TypeDocument { get; set; }

        public IEnumerable<long> UserIds { get; set; }

        public override string ToString()
        {
            return $"id : {Id} blobName: {BlobName}";
        }

        public string MetaContent => string.IsNullOrEmpty(Content) ? null : Content.RemoveEndOfString(200);
    }
}