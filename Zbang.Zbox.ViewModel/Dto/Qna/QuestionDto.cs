using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{ 
    public class QuestionDto
    {
        private DateTime m_Date;
        public QuestionDto()
        {
            Answers = new List<AnswerDto>();
            Files = new List<ItemDto>();
        }
        public Guid Id { get; set; }

        public string UserImage { get; set; }
        public string UserName { get; set; }
        public long UserUid { get; set; }

        public string Content { get; set; }

        public List<AnswerDto> Answers { get; set; }

        public List<ItemDto> Files { get; set; }

        public string Url { get; set; }

        public DateTime CreationTime
        {
            get { return m_Date; }
            set
            {
                m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
    }
}
