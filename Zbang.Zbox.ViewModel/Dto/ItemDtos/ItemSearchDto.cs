using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    //public class ItemSearchDto
    //{
    //    public long Id { get; set; }
    //    public string Image { get; set; }
    //    public string Name { get; set; }
        
    //    public string Content { get; set; }

    //    public string Url { get; set; }
    //    public string UniversityName { get; set; }
    //    public string BoxName { get; set; }
    //    public string BoxCode { get; set; }
    //    public string BoxProfessor { get; set; }

    //    public string Type { get; set; }

    //    public long? UniversityId { get; set; }

    //    public string BlobName { get; set; }

    //    public long BoxId { get; set; }

    //    public Language Language { get; set; }
    //    public IEnumerable<long> UserIds { get; set; }

    //    public override string ToString()
    //    {
    //        return $"id : {Id} blobName: {BlobName}";
    //    }
    //}


    public class ItemSearchDto
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }

        public string Content { get; set; }

        public string Url { get; set; }
        public string UniversityName { get; set; }
        public string BoxName { get; set; }
        public string BoxCode { get; set; }
        public string BoxProfessor { get; set; }

        public string Type { get; set; }

        public long? UniversityId { get; set; }

        public string BlobName { get; set; }

        public long BoxId { get; set; }

        public Language? Language { get; set; }
        public IEnumerable<ItemSearchUsers> UserIds { get; set; }
        public IEnumerable<ItemSearchTag> Tags { get; set; }

        public override string ToString()
        {
            return $"id : {Id} blobName: {BlobName}";
        }
    }

    public class ItemSearchUsers
    {
        public long Id { get; set; }
    }

    public class ItemSearchTag
    {
        public string Name { get; set; }
    }
}