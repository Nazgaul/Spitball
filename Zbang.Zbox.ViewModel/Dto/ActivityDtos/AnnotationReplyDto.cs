using System;

namespace Zbang.Zbox.ViewModel.Dto.ActivityDtos
{
    public class AnnotationReplyDto
    {
        private DateTime _date;

        public long Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreationDate
        {
            get { return _date; }
            set
            {
                _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public string UserName { get; set; }
        public long ParentId { get; set; }

        public long UserId { get; set; }

        public string UserImage { get; set; }
    }
}