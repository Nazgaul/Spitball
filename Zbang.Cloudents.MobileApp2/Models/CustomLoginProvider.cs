using System;
using System.Security.Claims;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Zbox.Infrastructure.Consts;

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
        }

        public override ProviderCredentials CreateCredentials(ClaimsIdentity claimsIdentity)
        {
            if (claimsIdentity == null)
            {
                throw new ArgumentNullException("claimsIdentity");
            }

            string username = claimsIdentity.FindFirst(ClaimConsts.UserIdClaim).Value;


            var claimUserId = claimsIdentity.FindFirst(ClaimConsts.UserIdClaim);
            var claimUniversityId = claimsIdentity.FindFirst(ClaimConsts.UniversityIdClaim);
            var claimUniversityDataId = claimsIdentity.FindFirst(ClaimConsts.UniversityDataClaim);

            if (claimUserId == null)
            {
                throw new NullReferenceException("userid can not be null");
            }
            long? universityId = null, universityDataId = null;

            if (claimUniversityId != null)
            {
                long val;
                if (long.TryParse(claimUniversityId.Value, out val))
                {
                    universityId = val;
                }
            }
            if (claimUniversityDataId != null)
            {
                long val;
                if (long.TryParse(claimUniversityDataId.Value, out val))
                {
                    universityDataId = val;
                }
            }

            var credentials = new CustomLoginProviderCredentials
            {
                UserId = TokenHandler.CreateUserId(Name, username),
                UniversityId =   universityId,
                UniversityDataId = universityDataId,
                CUserId = long.Parse(claimUserId.Value)
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
        //public override System.Threading.Tasks.Task<Microsoft.Owin.Security.AuthenticateResult> AuthenticateClient(System.Net.Http.HttpRequestMessage request)
        //{
        //    return base.AuthenticateClient(request);
        //}

        //public override LoginResult CreateLoginResult(ClaimsIdentity claimsIdentity, string secretKey)
        //{
        //    return base.CreateLoginResult(claimsIdentity, secretKey);
        //}

        //protected override TokenInfo CreateTokenInfo(ClaimsIdentity claimsIdentity, ProviderCredentials credentialsClaim, string secretKey)
        //{
        //    return base.CreateTokenInfo(claimsIdentity, credentialsClaim, secretKey);
        //}
    }
}