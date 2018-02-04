using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class AiRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Sentence { get; set; }
    }
}
