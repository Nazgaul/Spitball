using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class DocumentSearchDto : ItemSearchDto
    {
        
        private IEnumerable<ItemType> m_Type;
        public string Image { get; set; }

        // public override string Content { get; set; }
        public string DocumentContent { get; set; }

        public override string Name => Path.GetFileNameWithoutExtension(FileName);
        public override string Content => DocumentContent;

        public string FileName { get; set; }
        public override IEnumerable<ItemType> Type
        {
            get
            {
                if (m_Type != null && m_Type.Any())
                {
                    return m_Type;
                }
                var type = new List<ItemType>
                {
                    TypeDocument.ToLowerInvariant() == "file" ? ItemType.Document : ItemType.Link
                };
              
                if (DocType.Equals(ItemType.Undefined) && string.IsNullOrEmpty(Content))
                {
                    return type;
                }            
                type.Add(DocType);
                m_Type = type;
                return m_Type;
            }

        }

        public override string SearchContentId => "item_" + Id;

        public string Url { get; set; }

        //public long? UniversityId { get; set; }

        public ItemType DocType { get; set; }

        public string TypeDocument { get; set; }

        public IEnumerable<long> UserIds { get; set; }

        public override string ToString()
        {
            return $"id : {Id} blobName: {BlobName}";
        }

        public override string[] MetaContent
        {
            get
            {
                if (string.IsNullOrEmpty(Content))
                {
                    return null;
                }
                return new[] { Content.RemoveEndOfString(200) };
            }
        }

        
        public override int? ContentCount => null;
    }
}