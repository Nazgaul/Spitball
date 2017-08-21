using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class FlashcardSearch
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BoxName { get; set; }
        public string[] Front { get; set; }
        public string MetaContent { get; set; }
        public string[] Back { get; set; }
        public string[] UserId { get; set; }
        public string UniversityName { get; set; }
        public long? BoxId { get; set; }
        public string UniversityId { get; set; }
    }
}