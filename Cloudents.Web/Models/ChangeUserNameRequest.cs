using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ChangeUserNameRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}
