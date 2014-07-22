using System;

namespace Zbang.Zbox.ViewModel.Dto
{
    [Serializable]
    public class InviteDto
    {
        private DateTime m_Date;
        public Guid MsgId { get; set; }
        public long? BoxId { get; set; }
        public string UserPic { get; set; }
        public string UserName { get; set; }
        public DateTime Date
        {
            get { return m_Date; }
            set
            {
                m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }

        public string Message { get; set; }

        public string BoxName { get; private set; }

        public string Url { get; set; }
    }
}
