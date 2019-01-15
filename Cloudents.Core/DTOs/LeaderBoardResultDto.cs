using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class LeaderBoardResultDto
    {
        public long SBL { get; set; }
        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }
    }
}
