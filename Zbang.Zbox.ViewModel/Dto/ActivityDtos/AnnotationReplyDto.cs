using System;

namespace Zbang.Zbox.ViewModel.Dto.ActivityDtos
{
    public class AnnotationReplyDto
    {
        private DateTime m_Date;

        public long Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreationDate
        {
            get { return m_Date; }
            set
            {
                m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public string UserImage { get; set; }
        public string UserName { get; set; }
        public long ParentId { get; set; }


    }
}