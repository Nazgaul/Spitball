using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Identity
{
    public sealed class SbUserManager : UserManager<User>
    {
        private readonly ISmsProvider _smsProvider;
        public SbUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            ISmsProvider smsProvider) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                keyNormalizer, errors, services, logger)
        {
            _smsProvider = smsProvider;
        }

        public async Task<IdentityResult> SetPhoneNumberAndCountryAsync(User user, string phoneNumber, string countryCallingCode, CancellationToken cancellationToken)
        {
            var result = await _smsProvider.ValidateNumberAsync(phoneNumber, countryCallingCode, cancellationToken);
            if (string.IsNullOrEmpty(result.phoneNumber))
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "InvalidPhoneNumber",
                    Description = "InvalidPhoneNumber"
                });
            }

            user.ChangeCountry(result.country);
            return await SetPhoneNumberAsync(user, result.phoneNumber);
        }
    }
}