using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace Zbang.Cloudents.MobileApp2.Models
{
    public class CustomLoginProviderCredentials : ProviderCredentials
    {
        public CustomLoginProviderCredentials()
            : base(CustomLoginProvider.ProviderName)
        {
        }
    }
}