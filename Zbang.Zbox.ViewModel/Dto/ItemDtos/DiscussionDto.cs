using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class DiscussionDto
    {
        private DateTime _date;

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set { _date = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }
        public string UserPicture { get; set; }
        public long UserId { get; set; }
        //public string UserUrl { get; set; }
        public string Text { get; set; }
        public Guid QuestionId { get; set; }
    }
}
