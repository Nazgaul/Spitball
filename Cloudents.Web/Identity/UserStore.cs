using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using NHibernate.Exceptions;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public sealed class UserStore :
        IUserStore<User>,
        //IUserPasswordStore<User>,
        IUserEmailStore<User>,
        IUserTwoFactorStore<User>,
        IUserSecurityStampStore<User>, // need to create token for sms
        IUserPhoneNumberStore<User>
    {
        //private readonly Lazy<IRepository<User>> _userRepository;
        private readonly ICommandBus _bus;
        private readonly IQueryBus _queryBus;

        public UserStore(ICommandBus bus, IQueryBus queryBus)
        {
            _bus = bus;
            _queryBus = queryBus;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Name);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.Name = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var command = new CreateUserCommand(user);
            await _bus.DispatchAsync(command, cancellationToken).ConfigureAwait(false);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateUserCommand(user);
                await _bus.DispatchAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (GenericADOException ex)
            {
                if (ex.InnerException is SqlException sql && sql.Number == 2601)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Duplicate",
                        Code = "Duplicate"
                    });
                }

            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var p = long.Parse(userId);
            Expression<Func<User, bool>> expression = f => f.Id == p;
            return _queryBus.QueryAsync<Expression<Func<User, bool>>, User>(expression, cancellationToken);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            // try
            //{
            //nhibernate flush before query base on flushmode.auto

            Expression<Func<User, bool>> expression = s => s.NormalizedName == normalizedUserName;
            return _queryBus.QueryAsync<Expression<Func<User, bool>>, User>(expression, cancellationToken);


            //return await _userRepository.Value.GetQueryable()
            //    .SingleOrDefaultAsync(s => s.NormalizedName == normalizedUserName,
            //        cancellationToken).ConfigureAwait(false);
            //}
            //catch (GenericADOException ex)
            //{
            //    if (ex.InnerException is SqlException sql && sql.Number == 2627)
            //    {
            //        throw new UserNameExistsException("user exists", ex);
            //    }

            //    throw;
            //}

        }

        //public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        //{
        //    user.PublicKey = passwordHash;
        //    return Task.CompletedTask;
        //}

        //public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.PublicKey);
        //}

        //public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(!string.IsNullOrEmpty(user.PublicKey));
        //}

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            Expression<Func<User, bool>> expression = f => f.NormalizedEmail == normalizedEmail;
            return _queryBus.QueryAsync<Expression<Func<User, bool>>, User>(expression, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(User user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }
    }
}
