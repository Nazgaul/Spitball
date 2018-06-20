using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Auth
{
    [UsedImplicitly]
    public sealed class GoogleAuth : IGoogleAuth
    {
        private readonly IMapper _mapper;
        private readonly IRestClient _client;

        public GoogleAuth(IMapper mapper, IRestClient client)
        {
            _mapper = mapper;
            _client = client;
        }


        public async Task<AuthDto> LogInAsync(string token, CancellationToken cancellationToken)
        {
           var result = await _client.GetAsync<GoogleToken>(new Uri("https://www.googleapis.com/oauth2/v3/tokeninfo"), new NameValueCollection()
            {
                ["id_token"] = token
            }, cancellationToken).ConfigureAwait(false);


            
            //return Task.FromResult<AuthDto>(null);
            //var result = await GoogleJsonWebSignature.ValidateAsync(token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }

            if (string.Equals(result.EmailVerified, "false", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!string.Equals(result.Aud,
                "341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return _mapper.Map<AuthDto>(result);
        }
    }


    public class GoogleToken
    {
        //public string Azp { get; set; }
        public string Aud { get; set; }
       // public string Sub { get; set; }
        public string Email { get; set; }
        [JsonProperty("email_verified")]
        public string EmailVerified { get; set; }
       // public string AtHash { get; set; }
        //public string Exp { get; set; }
       // public string Iss { get; set; }
        //public string Jti { get; set; }
       // public string Iat { get; set; }
        public string Name { get; set; }
       // public string Picture { get; set; }
        [JsonProperty("given_name")]

        public string GivenName { get; set; }
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
        //public string Locale { get; set; }
       // public string Alg { get; set; }
        //public string Kid { get; set; }
    }

}
