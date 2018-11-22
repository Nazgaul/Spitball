namespace Cloudents.Core.DTOs.Admin
{
    public class PendingQuestionDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }

        public PendingQuestionDto(long id, long userId, string text, string email, int imagesCount)
        {
            Id = id;
            UserId = userId;
            Text = text;
            Email = email;
            ImagesCount = imagesCount;
        }

        public int ImagesCount { get; set; }
    }

    public class FictivePendingQuestionDto
    {
        public FictivePendingQuestionDto(long id)
        {
            Id = id;
        }

        public long Id { get;  }
    }
}
