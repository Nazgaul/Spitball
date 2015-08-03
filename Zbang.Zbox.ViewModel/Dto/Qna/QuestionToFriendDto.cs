using System;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class QuestionToFriendDto
    {
        public string BoxName { get; set; }
        public string Content { get; set; }
        public long BoxId { get; set; }
        public int AnswersCount { get; set; }

        public Guid? Id { get; set; }

        public string Url { get; set; }
    }

}
