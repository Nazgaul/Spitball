using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;

namespace Zbang.Zbox.ViewModel.Dto.UserDtos
{
    public class UserNotification
    {
        public EmailSend EmailNotification { get; set; }
        public IEnumerable<BoxNotificationDto> BoxNotifications { get; set; }
    }
}
