using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{ 
    public class QuestionDto
    {
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

        public DateTime CreationTime { get; set; }
    }
}
