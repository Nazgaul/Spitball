using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateStudyRoomRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public IEnumerable<long> UserId { get; set; }

        [Required] 
        public decimal Price { get; set; }
    }
}