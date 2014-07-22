using System.Collections.Generic;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Qna;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserToFriendActivity
    {
        public IEnumerable<QuestionToFriendDto> Questions { get; set; }
        public IEnumerable<AnswerToFriendDto> Answers { get; set; }
        public IEnumerable<ItemToFriendDto> Items { get; set; }
    }
}
