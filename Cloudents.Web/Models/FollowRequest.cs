using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Models
{
    public class FollowRequest
    {
        [Required]
        public long Id { get; set; }
    }
}
