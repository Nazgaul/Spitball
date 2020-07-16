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
        private readonly IPhoneValidator _smsProvider;


        public SbUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            IPhoneValidator smsProvider) :
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

            try
            {
                user.ChangeCountry(result.country);
            }
            catch (NotSupportedException)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "CountryNotSupported",
                    Description = "CountryNotSupported"
                });
            }

            user.PhoneNumber = result.phoneNumber;
            user.PhoneNumberConfirmed = false;
            return await UpdateAsync(user);
        }

      

        public override async Task<IdentityResult> ChangePhoneNumberAsync(User user, string phoneNumber, string token)
        {
            var x = await _smsProvider.VerifyCodeAsync(phoneNumber, token, default);
            if (x)
            {
                var store = (IUserPhoneNumberStore<User>)Store;
                await store.SetPhoneNumberConfirmedAsync(user, true, CancellationToken);
                return await UpdateUserAsync(user);
            }
            return await base.ChangePhoneNumberAsync(user, phoneNumber, token);
        }
    }
}