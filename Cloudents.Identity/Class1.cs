﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Identity
{
    public sealed class UserStore :
        IUserEmailStore<RegularUser>,
        IUserPasswordStore<RegularUser>,
        IUserTwoFactorStore<RegularUser>,
        IUserSecurityStampStore<RegularUser>, // need to create token for sms
        IUserPhoneNumberStore<RegularUser>,
        IUserAuthenticatorKeyStore<RegularUser>,
        IUserLockoutStore<RegularUser>,
        IUserLoginStore<RegularUser>//,
        //IUserRoleStore<RegularUser>
    {
        private readonly ISession _session;
        private readonly ICommandBus _bus;
        //private readonly IQueryBus _queryBus;

        public UserStore(ICommandBus bus, ISession session)
        {
            _bus = bus;
            _session = session;
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public Task<string> GetUserIdAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }



        public Task<string> GetUserNameAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Name);
        }

        public Task SetUserNameAsync(RegularUser user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedName);
        }

        public Task SetNormalizedUserNameAsync(RegularUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(RegularUser user, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand(user);
            await _bus.DispatchAsync(command, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(RegularUser user, CancellationToken cancellationToken)
        {
            try
            {
                var command = new UpdateUserCommand(user);
                await _bus.DispatchAsync(command, cancellationToken);
            }
            catch (DuplicateRowException)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Duplicate",
                    Code = "Duplicate"
                });

            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(RegularUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RegularUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var p = long.Parse(userId);
            return _session.LoadAsync<RegularUser>(p, cancellationToken);
        }

        public async Task<RegularUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            //Expression<Func<RegularUser, bool>> expression = s => s.NormalizedName == normalizedUserName;
            return _session.Query<RegularUser>().FirstOrDefault(w => w.NormalizedName == normalizedUserName);
            //return _queryBus.QueryAsync(new UserDataExpressionQuery(expression), cancellationToken);
        }

        public Task SetEmailAsync(RegularUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(RegularUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<RegularUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            //Expression<Func<RegularUser, bool>> expression = f => f.NormalizedEmail == normalizedEmail;
            return await _session.Query<RegularUser>().FirstOrDefaultAsync(w => w.NormalizedEmail == normalizedEmail, cancellationToken: cancellationToken);
            //return _queryBus.QueryAsync(new UserDataExpressionQuery(expression), cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(RegularUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(RegularUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetPhoneNumberAsync(RegularUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(RegularUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(RegularUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetAuthenticatorKeyAsync(RegularUser user, string key, CancellationToken cancellationToken)
        {
            user.AuthenticatorKey = key;
            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticatorKeyAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AuthenticatorKey);
        }

        public Task SetPasswordHashAsync(RegularUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);

        }

        public Task<bool> HasPasswordAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(RegularUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(RegularUser user, CancellationToken cancellationToken)
        {
            int accessFailedCount = user.AccessFailedCount;
            user.AccessFailedCount = accessFailedCount + 1;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(RegularUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(RegularUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(RegularUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public async Task AddLoginAsync(RegularUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var command =
                new AddUserLoginCommand(user, login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);

            await _bus.DispatchAsync(command, cancellationToken);

        }

        public Task RemoveLoginAsync(RegularUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(RegularUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RegularUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return _session.Query<UserLogin>()
                .Fetch(f => f.User)
                .Where(w => w.ProviderKey == providerKey && w.LoginProvider == loginProvider)
                .Select(s => s.User).SingleOrDefaultAsync(cancellationToken: cancellationToken);
            //return _queryBus.QueryAsync(new UserLoginQuery(loginProvider, providerKey), cancellationToken);
        }


        public async Task AddToRoleAsync(RegularUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveFromRoleAsync(RegularUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(RegularUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //public Task<bool> IsInRoleAsync(RegularUser user, string roleName, CancellationToken cancellationToken)
        //{
        //    var result = user.UserRoles.Any(f => f.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        //    return Task.FromResult(result);
        //}

        public async Task<IList<RegularUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }




}
