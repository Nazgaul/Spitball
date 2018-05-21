using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs
{
    public class QuestionDetailDto
    {
        //public QuestionDetailDto()
        //{
        //    User = new QuestionDetailUserDto();
        //}
        public string Subject { get; set; }

        public long Id { get; set; }

        public string Text { get; set; }

        public decimal Price { get; set; }

        public UserDto User { get; set; }
        public IEnumerable<QuestionDetailAnswerDto> Answers { get; set; }

        public DateTime Create { get; set; }

    }

    public class UserDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class QuestionDetailAnswerDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public UserDto User { get; set; }
        public DateTime Create { get; set; }

    }
}
