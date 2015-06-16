using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    class BoxSearch
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Professor { get; set; }
        public string Course { get; set; }
        public string Url { get; set; }
        public long? UniversityId { get; set; }
        public string[] UserId { get; set; }
        public string[] Department { get; set; }

        public int? Type { get; set; }


    }
}