using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class ApproveTutorRequest
    {
        [Required]
        public long Id { get; set; }
    }
}
