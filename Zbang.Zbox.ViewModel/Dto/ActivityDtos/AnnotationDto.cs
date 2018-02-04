using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto.ActivityDtos
{
    public class AnnotationDto
    {
        private DateTime _date;
        public AnnotationDto()
        {
            Replies = new List<AnnotationReplyDto>();
        }
        public long Id { get; set; }

        public string Comment { get; set; }

        public DateTime CreationDate
        {
            get => _date;
            set => _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public string UserName { get; set; }
        public string UserImage { get; set; }
        public long UserId { get; set; }

        public List<AnnotationReplyDto> Replies { get; set; }
    }
}
