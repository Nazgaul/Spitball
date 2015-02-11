namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class AnswerToFriendDto
    {
        
        public long Boxid { get; set; }
        public string BoxName { get; set; }
        public long QUserId { get; set; }
        public string QContent { get; set; }
        public string QUserImage { get; set; }
        public string QUserName { get; set; }
        public string Content { get; set; }
        public int AnswersCount { get; set; }


        public string Url { get; set; }

    }
}
