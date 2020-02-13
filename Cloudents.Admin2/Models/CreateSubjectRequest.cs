using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateSubjectRequest
    {
        [RegularExpression(@"[A-Za-z0-9|\s!@#$%^&*()_+-]*")]
        [StringLength(300)]
        public string EnSubjectName { get; set; }
        [Required]
        [RegularExpression(@"[א-ת0-9|\s!@#$%^&*()_+-]*")]
        [StringLength(150)]
        public string HeSubjectName { get; set; }
    }
}
