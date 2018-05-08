using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Google.Apis.Oauth2.v2;

namespace Cloudents.Infrastructure.Auth
{
    public sealed class GoogleAuth : IGoogleAuth, IDisposable
    {
        private readonly Oauth2Service _service;

        public GoogleAuth()
        {
            _service = new Oauth2Service();
        }


        public async Task<string> LogInAsync(string token, CancellationToken cancellationToken)
        {
            //var request = new Google.Apis.Oauth2.v2.UserinfoResource();
            //var tokeninfoRequest = _service.Tokeninfo();
            //tokeninfoRequest.IdToken = token;

            
            //var result = await tokeninfoRequest.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            
            //if (result == null)
            //{
            //    return null;
            //}
            //if (result.aud)

            //if (result.VerifiedEmail.GetValueOrDefault())
            //{
                return null;
            //}
            //result.
            //return result.Email;

        }

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}
