
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs.BoxDtos
{

    public class BoxNotificationDto
    {


        public BoxNotificationDto(long id, string name, string owner, NotificationSettings notification,BoxType boxType)
        {
            Id = id;
            Name = name;
            Owner = owner;
            Notifications = notification;
            BoxType = boxType;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }

        public NotificationSettings Notifications { get; set; }

        public BoxType BoxType { get; set; }

        public string Url { get; set; }
    }
}
