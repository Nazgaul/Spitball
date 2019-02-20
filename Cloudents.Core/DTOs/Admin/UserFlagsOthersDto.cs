using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserFlagsOthersDto
    {
        public long UserId { get; set; }
        public string Country { get; set; }
        public int Flags { get; set; }
        public static int Rows { get; set; }
    }
}
