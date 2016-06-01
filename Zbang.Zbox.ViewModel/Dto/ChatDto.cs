using System;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class ChatDto
    {
        private DateTime m_Date;
        public string Text { get; set; }
        public DateTime Time
        {
            get { return m_Date; }
            set
            {
                m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public long UserId { get; set; }

        public string Blob { get; set; }

        public Guid Id { get; set; }
    }
}
