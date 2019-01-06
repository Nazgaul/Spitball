using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class LeaderBoardDto
    {
        public long Id { get; set; }
        public string Name  { get; set; }
        public long Score { get; set; }
        public string University { get; set; }
    }
}
