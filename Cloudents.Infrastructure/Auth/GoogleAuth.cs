using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

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


        public Task<AuthDto> LogInAsync(string token, CancellationToken cancellationToken)
        {
            return Task.FromResult<AuthDto>(null);
            //var result = await GoogleJsonWebSignature.ValidateAsync(token).ConfigureAwait(false);
            //if (result == null)
            //{
            //    return null;
            //}

            //if (!result.EmailVerified)
            //{
            //    return null;
            //}

            //if (!string.Equals(result.Audience.ToString(),
            //    "341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com", StringComparison.OrdinalIgnoreCase))
            //{
            //    return null;
            //}

            //return _mapper.Map<AuthDto>(result);
        }
    }
}
