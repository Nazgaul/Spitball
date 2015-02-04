using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace Zbang.Cloudents.MobileApp.Models
{
    public class CustomLoginProviderCredentials : ProviderCredentials
    {
        public CustomLoginProviderCredentials()
            : base(CustomLoginProvider.ProviderName)
        {
        }
    }
}