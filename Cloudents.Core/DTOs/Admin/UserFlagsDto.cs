using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserFlagsDto
    {
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string State { get; set; }
        public string FlagReason { get; set; }
        public int VoteCount { get; set; }
        public char ItemType { get; set; }
    }
}
