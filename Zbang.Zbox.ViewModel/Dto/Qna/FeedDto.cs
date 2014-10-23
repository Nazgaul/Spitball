
using System.Collections.Generic;


namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class FeedDto
    {
        public IEnumerable<BoxDtos.RecommendBoxDto> RecommendBoxes { get; set; }
        public IEnumerable<QuestionDto> Feed { get; set; }
    }
}
