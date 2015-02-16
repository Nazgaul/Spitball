using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemSearchDto
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        
        public string Content { get; set; }

        public string Url { get; set; }
        public string UniversityName { get; set; }
        public string BoxName { get; set; }

        public double Rate { get; set; }
        public int Views { get; set; }

        public long? UniversityId { get; set; }

        public string BlobName { get; set; }

        public long BoxId { get; set; }

        public IEnumerable<long> UserIds { get; set; }

        public override string ToString()
        {
            return string.Format("id : {0} blobName: {1}", Id, BlobName);
        }
    }
}