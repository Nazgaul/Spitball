using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Google.Apis.Auth;

namespace Cloudents.Infrastructure.Auth
{
    public sealed class GoogleAuth : IGoogleAuth
    {
        private readonly IMapper _mapper;

        public GoogleAuth(IMapper mapper)
        {
            _mapper = mapper;
        }


        public async Task<AuthDto> LogInAsync(string token, CancellationToken cancellationToken)
        {
            var result = await GoogleJsonWebSignature.ValidateAsync(token).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }

            if (!result.EmailVerified)
            {
                return null;
            }

            if (!string.Equals(result.Audience.ToString(),
                "997823384046-ddhrphigu0hsgkk1dglajaifcg2rggbm.apps.googleusercontent.com", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return _mapper.Map<AuthDto>(result);
        }
    }
}
