namespace Zbang.Zbox.ViewModel.Dto.Emails
{
    public class QuizDiscussionDigestDto
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public long UserId { get; set; }

        public string QuizName { get; set; }
        public long QuizId { get; set; }

        public string Url { get; set; }
    }

}
