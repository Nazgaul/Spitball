using System;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class ActivityDto
    {
        public long BoxId { get; set; }
        public string Content { get; set; }
        public Guid Id { get; set; }
        public string Type { get; set; }

        public string BoxName { get; set; }

        public Guid? PostId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid DepartmentId { get; set; }

        public string Url { get; set; }
    }
}
