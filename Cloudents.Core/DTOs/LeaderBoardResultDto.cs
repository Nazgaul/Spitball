using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class LeaderBoardResultDto
    {
        [DataMember(Name = "SBL")]
        public long Points { get; set; }
        [DataMember]
        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }
    }
}
