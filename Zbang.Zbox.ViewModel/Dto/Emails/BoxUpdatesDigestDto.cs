using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Emails
{
    public class BoxUpdatesDigestDto
    {
        public IEnumerable<ItemDigestDto> Items { get; set; }

        public IEnumerable<QuizDigestDto> Quizzes { get; set; }

        public IEnumerable<QuizDiscussionDigestDto> QuizDiscussions { get; set; }

        public IEnumerable<QnADigestDto> Comments { get; set; }

        public IEnumerable<QnADigestDto> Replies { get; set; }

        public IEnumerable<BoxDigestDto> Boxes { get; set; }
    }

    public class UserUpdatesDigestDto
    {
        public long BoxId { get; set; }
        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }
        public long? ItemId { get; set; }
        public long? QuizId { get; set; }
        public Guid? QuizDiscussionId { get; set; }
    }
}
