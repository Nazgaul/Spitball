using Cloudents.Core.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Admin2.Models
{
    public class UsersFlagsResponse
    {
        public IEnumerable<UserFlagsOthersDto> flags { get; set; }
        public int? Rows { get; set; }
    }
}
