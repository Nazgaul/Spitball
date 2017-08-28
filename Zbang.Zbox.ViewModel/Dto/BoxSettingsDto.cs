
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto
{
    public class BoxSettingsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public BoxPrivacySetting PrivacySetting { get; set; }
        public NotificationSetting NotificationSetting { get; set; }
        public UserRelationshipType UserType { get; set; }
    }
}
