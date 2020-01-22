
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetUserGradeRequest
    {
        [Required]
        [Range(1, 12)]
        public short Grade { get; set; }
    }
}
