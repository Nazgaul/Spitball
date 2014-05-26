using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.ActivityDtos
{
    public class AnnotationDto
    {
        private DateTime m_Date;
        public AnnotationDto()
        {
            Replies = new List<AnnotationReplyDto>();
        }
        public long Id { get; set; }
        public int ImageId { get; set; }

        public string Comment { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

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
        public string Uid { get; set; }

        public List<AnnotationReplyDto> Replies { get; set; }
    }
    public class AnnotationReplyDto
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreationDate { get; set; }
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public long ParentId { get; set; }


    }
}
