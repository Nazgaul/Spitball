using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class LeaderBoardResultDto
    {
        public long SBL { get; set; }
        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }
    }
}
