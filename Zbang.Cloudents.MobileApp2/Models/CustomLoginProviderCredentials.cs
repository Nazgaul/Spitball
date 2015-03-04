using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace Zbang.Cloudents.MobileApp2.Models
{
    public class CustomLoginProviderCredentials : ProviderCredentials
    {
        public CustomLoginProviderCredentials()
            : base(CustomLoginProvider.ProviderName)
        {
        }

        public long CUserId { get; set; }
        public long? UniversityId { get; set; }
        public long? UniversityDataId { get; set; }
    }
}