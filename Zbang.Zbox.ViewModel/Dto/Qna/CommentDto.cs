using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.Qna
{
    public class CommentDto
    {
        private DateTime m_Date;

        public Guid Id { get; set; }

        public string UserImage { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }

        public string Content { get; set; }

        public IEnumerable<ReplyDto> Replies { get; set; }

        public IEnumerable<ItemDto> Files { get; set; }

        public int Badges { get; set; }
        public int Score { get; set; }
        public int RepliesCount { get; set; }

        public int LikesCount { get; set; }

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
