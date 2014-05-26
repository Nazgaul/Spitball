using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class UserToFriendActivity
    {
        public IEnumerable<Zbang.Zbox.ViewModel.DTOs.Qna.QuestionToFriendDto> Questions { get; set; }
        public IEnumerable<Zbang.Zbox.ViewModel.DTOs.Qna.AnswerToFriendDto> Answers { get; set; }
        public IEnumerable<Zbang.Zbox.ViewModel.DTOs.ItemDtos.ItemToFriendDto> Items { get; set; }
    }
}
