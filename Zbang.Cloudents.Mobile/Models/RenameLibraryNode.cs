using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mobile.Models
{
    public class RenameLibraryNode
    {
        [Required]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string NewName { get; set; }
    }
}