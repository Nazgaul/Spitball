using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class CodeRequest
    {
        [Required]
        public string Number { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}";
        }
    }
}