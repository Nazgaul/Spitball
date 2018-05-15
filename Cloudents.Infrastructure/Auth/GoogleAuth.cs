using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Google.Apis.Auth;
using JetBrains.Annotations;
using IMapper = Cloudents.Core.Interfaces.IMapper;

namespace Cloudents.Infrastructure.Auth
{
    [UsedImplicitly]
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
                "341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return _mapper.Map<AuthDto>(result);
        }
    }
}
