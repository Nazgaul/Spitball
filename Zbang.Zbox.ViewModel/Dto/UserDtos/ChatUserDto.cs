using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class ChatUserDto
    {
        public long Id { get; set; }
        public string Image { get; set; }

        public string Name { get; set; }

        public bool Online { get; set; }

        public Guid? Conversation { get; set; }

        public int? Unread { get; set; }

        private DateTime m_LastSeen;

        public int Section { get; set; }

        public DateTime LastSeen { get { return m_LastSeen; } set { m_LastSeen = DateTime.SpecifyKind(value, DateTimeKind.Utc); } }
    }
}