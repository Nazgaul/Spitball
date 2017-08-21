using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class GoogleUserData
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Locale { get; set; }

        [JsonProperty(PropertyName="sub")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "given_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "family_name")]
        public string LastName { get; set; }

    }
}
