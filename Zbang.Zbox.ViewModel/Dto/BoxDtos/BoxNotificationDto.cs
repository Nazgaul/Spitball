using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{

    public class BoxNotificationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public NotificationSettings Notifications { get; set; }
        public string Url { get; set; }
    }
}
