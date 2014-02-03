
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.DTOs
{
    public class BoxSettingsDto 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public BoxPrivacySettings PrivacySetting { get; set; }
        public NotificationSettings NotificationSetting { get; set; }
        public UserRelationshipType UserType { get; set; }
    }
}
