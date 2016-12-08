using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class ReplyDto
    {
        private DateTime m_Date;
        public ReplyDto()
        {
           // Files = new List<ItemDto>();
        }
        public Guid Id { get; set; }
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }

        public string Content { get; set; }
        public Guid QuestionId { get; set; }
        public DateTime CreationTime { get { return m_Date; }
            set
            {
                m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
        public int LikesCount { get; set; }
        public IEnumerable<ItemDto> Files { get; set; }

        public int Badges { get; set; }
        public int Score { get; set; }

    }
}
