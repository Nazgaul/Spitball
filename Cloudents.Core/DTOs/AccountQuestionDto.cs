using System;

namespace Cloudents.Core.DTOs
{
    public class AccountQuestionDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public QuestionUserDto User { get; set; }
    }
}
