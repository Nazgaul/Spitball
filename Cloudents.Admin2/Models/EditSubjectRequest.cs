using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class EditSubjectRequest 
    {
        [Required]
        public long SubjectId { get; set; }

        [StringLength(300)]
        [Required]
        public string Name { get; set; }


    }
}
