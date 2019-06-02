using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class AnonymousRequestTutorRequest
    {
        public string Text { get; set; }
       // public string Course { get; set; }

        [JsonProperty("mail")]
        public string Email { get; set; }
        public string Name { get; set; }
       // public string University { get; set; }
        [JsonProperty("phone")]
        public string PhoneNumber { get; set; }
        [MaxLength(4, ErrorMessage = "MaxLength")]
        public string[] Files { get; set; }

    }
}
