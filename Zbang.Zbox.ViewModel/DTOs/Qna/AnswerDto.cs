using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.Qna
{
    public class AnswerDto
    {
        private DateTime m_Date;
        public AnswerDto()
        {
            Files = new List<ItemDto>();
        }
        public Guid Id { get; set; }
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }

        public string Content { get; set; }
        public Guid QuestionId { get; set; }
        public bool Answer { get; set; }
        public int Rating { get; set; }
        public bool IRate { get; set; }
        public DateTime CreationTime { get { return m_Date; }
            set
            {
                m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public List<ItemDto> Files { get; set; }

        public string Url { get; set; }

      
    }
}
