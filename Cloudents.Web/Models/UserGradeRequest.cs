using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class UserGradeRequest
    {

        [Required]
        [Range(1, 12)]
        public short Grade { get; set; }

    }
}
