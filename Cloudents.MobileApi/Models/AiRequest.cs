using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace Cloudents.Api.Models
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