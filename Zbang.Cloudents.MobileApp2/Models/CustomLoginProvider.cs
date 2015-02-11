using System;
using System.Security.Claims;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace Zbang.Cloudents.MobileApp2.Models
{
    public class CustomLoginProvider : LoginProvider
    {
        public const string ProviderName = "cloudents";

        public override string Name
        {
            get { return ProviderName; }
        }

        public CustomLoginProvider(IServiceTokenHandler tokenHandler)
            : base(tokenHandler)
        {
            TokenLifetime = new TimeSpan(30, 0, 0, 0);
        }

        public override void ConfigureMiddleware(Owin.IAppBuilder appBuilder,
            Microsoft.WindowsAzure.Mobile.Service.ServiceSettingsDictionary settings)
        {
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(appBuilder, false);
            // Not Applicable - used for federated identity flows
            return;
        }

        public override ProviderCredentials CreateCredentials(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null)
            {
                throw new ArgumentNullException("claimsIdentity");
            }

            string username = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var credentials = new CustomLoginProviderCredentials
            {
                UserId = this.TokenHandler.CreateUserId(this.Name, username)
            };

            return credentials;
        }

       

        public override ProviderCredentials ParseCredentials(Newtonsoft.Json.Linq.JObject serialized)
        {
            if (serialized == null)
            {
                throw new ArgumentNullException("serialized");
            }

            return serialized.ToObject<CustomLoginProviderCredentials>();
        }
    }
}