using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Search
{
    [SerializePropertyNamesAsCamelCase]
    class UniversitySearch
    {
        /*  private const string IdField = "id";
        private const string NameField = "name";
        private const string ExtraOneField = "extra1";
        private const string ExtraTwoField = "extra2";
        private const string ImageField = "imageField";*/
        public string Id { get; set; }
        public string Name { get; set; }
        public string Extra1 { get; set; }
        public string Extra2 { get; set; }
        [JsonProperty("ImageField")]
        public string Image { get; set; }

        [JsonProperty("coutry")]
        public string Country { get; set; }
        public int? MembersCount { get; set; }

        public string[] MembersImages { get; set; }


    }
}