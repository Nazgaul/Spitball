using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class PendingQuestionDto
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }

        public int ImagesCount { get; set; }
    }

    public class PendingDocumentDto
    {
        public long Id { get; set; }
        public Uri Preview { get; set; }

        public string SiteLink { get; set; }
    }

    public class FictivePendingQuestionDto
    {
        public FictivePendingQuestionDto(long id)
        {
            Id = id;
        }

        public long Id { get;  }
    }

    public class PendingAnswerDto
    {
        public Guid Id { get; set; }

        public long UserId { get; set; }
        public string Text { get; set; }
        public string Email { get; set; }
    }
}
