using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class UserVotesDto
    {
        public DateTime Created { get; set; }
        public string ItemText { get; set; }
        public string ItemType { get; set; }
    }
}
