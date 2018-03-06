using System.ComponentModel.DataAnnotations;

namespace Cloudents.MobileApi.Models
{
    public class AiRequest
    {
        /// <summary>
        /// The sentence to break
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Sentence { get; set; }
    }
}