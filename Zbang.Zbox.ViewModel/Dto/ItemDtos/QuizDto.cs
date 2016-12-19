using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class QuizDto
    {
        private DateTime? m_Date;

        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Name { get; set; }
        public bool Publish { get; set; }
        public int NumOfViews { get; set; }
        public DateTime? Date
        {
            get { return m_Date; }
            set
            {
                if (value.HasValue)
                {
                    m_Date = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
                }
            }
        }

        public int Likes { get; set; }
        public string Url { get; set; } // Need in user page.
    }
}
