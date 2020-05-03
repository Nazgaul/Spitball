﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Mail;
using Cloudents.Query;
using Cloudents.Query.Users;

namespace Cloudents.Web.Identity
{
    public sealed class SbUserManager : UserManager<User>
    {
        private readonly IPhoneValidator _smsProvider;
        private readonly IQueryBus _queryBus;


        public SbUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            IPhoneValidator smsProvider, IQueryBus queryBus) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators,
                keyNormalizer, errors, services, logger)
        {
            _smsProvider = smsProvider;
            _queryBus = queryBus;
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

            return await SetPhoneNumberAsync(user, result.phoneNumber);
        }

        public async Task<User> FindByPhoneAsync(string phoneNumber, string countryCallingCode)
        {
            var phoneNumberWithCallingCode = TwilioProvider.BuildPhoneNumber(phoneNumber, countryCallingCode);
            Expression<Func<User, bool>> expression = s => s.PhoneNumber == phoneNumberWithCallingCode;
            return await _queryBus.QueryAsync(new UserDataExpressionQuery(expression), CancellationToken.None);

        }

        public override async Task<IdentityResult> ChangePhoneNumberAsync(User user, string phoneNumber, string token)
        {
            var x = await _smsProvider.VerifyCodeAsync(phoneNumber, token, default);
            if (x)
            {
                var store = (IUserPhoneNumberStore<User>)Store;
                await store.SetPhoneNumberConfirmedAsync(user, true, CancellationToken);
                //await UpdateSecurityStampInternal(user);
                return await UpdateUserAsync(user);
            }
            return await base.ChangePhoneNumberAsync(user, phoneNumber, token);
        }
    }
}