using System;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public class UpdatesDto
    {
        public long BoxId { get; set; }
        public Guid? QuestionId { get; set; }
        public Guid? AnswerId { get; set; }
        public long? ItemId { get; set; }
        public long? QuizId { get; set; }
    }
}
