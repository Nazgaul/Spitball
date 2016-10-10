using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.BoxDtos
{

    public class BoxNotificationDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public NotificationSetting Notifications { get; set; }
        public string Url { get; set; }

        public string UserName { get; set; }
    }
}
