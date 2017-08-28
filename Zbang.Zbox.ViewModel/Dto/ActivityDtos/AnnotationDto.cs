using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ActivityDtos
{
    public class AnnotationDto
    {
        private DateTime m_Date;
        public AnnotationDto()
        {
            Replies = new List<AnnotationReplyDto>();
        }
        public long Id { get; set; }

        public string Comment { get; set; }

        public DateTime CreationDate
        {
            get => m_Date;
            set => m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public string UserName { get; set; }
        public string UserImage { get; set; }
        public long UserId { get; set; }

        public List<AnnotationReplyDto> Replies { get; set; }
    }
}
