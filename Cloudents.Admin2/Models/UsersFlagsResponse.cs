using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;

namespace Cloudents.Admin2.Models
{
    public class UsersFlagsResponse
    {
        public IEnumerable<UserFlagsOthersDto> flags { get; set; }
        public int? Rows { get; set; }
    }
}
