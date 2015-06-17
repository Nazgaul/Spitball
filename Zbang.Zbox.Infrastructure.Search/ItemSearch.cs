using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class ItemSearch
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string BoxName { get; set; }
        public string Content { get; set; }
        public string MetaContent { get; set; }
        public string Url { get; set; }
        public string UniversityName { get; set; }
        public string UniversityId { get; set; }
        public string[] UserId { get; set; }
        public long? BoxId { get; set; }
        public string Extension { get; set; }
    }
}