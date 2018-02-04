using System;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class ChatDto
    {
        private DateTime _date;
        public string Text { get; set; }
        public DateTime Time
        {
            get { return _date; }
            set
            {
                _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public long UserId { get; set; }

        public string Blob { get; set; }

        public Guid Id { get; set; }
    }
}
