
using System.Collections.Generic;


namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class SideDto
    {
        public IEnumerable<BoxDtos.RecommendBoxDto> RecommendBoxes { get; set; }
        public IEnumerable<LeaderBoardDto> LeaderBoard { get; set; }
    }
}
