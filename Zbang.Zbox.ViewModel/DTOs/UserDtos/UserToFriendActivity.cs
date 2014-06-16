using System.Collections.Generic;
using Zbang.Zbox.ViewModel.DTOs.ItemDtos;
using Zbang.Zbox.ViewModel.DTOs.Qna;

namespace Zbang.Zbox.ViewModel.DTOs.UserDtos
{
    public class UserToFriendActivity
    {
        public IEnumerable<QuestionToFriendDto> Questions { get; set; }
        public IEnumerable<AnswerToFriendDto> Answers { get; set; }
        public IEnumerable<ItemToFriendDto> Items { get; set; }
    }
}
