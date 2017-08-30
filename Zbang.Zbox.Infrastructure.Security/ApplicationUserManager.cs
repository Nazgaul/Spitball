using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class ApplicationUserManager : UserManager<ApplicationUser>, IAccountService
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            PasswordHasher = new SqlPasswordHasher();
            // Configure validation logic for usernames
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = false;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            //manager.EmailService = new EmailService();
            //manager.SmsService = new SmsService();
            var dataProtectionProvider = Startup.DataProtectionProvider;

            if (dataProtectionProvider != null)
            {
                var protectorProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    TokenLifespan = new TimeSpan(30, 0, 0, 0)
                };

                UserTokenProvider = protectorProvider;
            }
        }
        //public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        //{
        //var manager = new UserManager(new UserStore<ApplicationUser>(context.Get<DbContext>()));
        //    // Configure validation logic for usernames
        //    manager.UserValidator = new UserValidator<ApplicationUser>(manager)
        //    {
        //        AllowOnlyAlphanumericUserNames = false,
        //        RequireUniqueEmail = true
        //    };

        //    // Configure validation logic for passwords
        //    manager.PasswordValidator = new PasswordValidator
        //    {
        //        RequiredLength = 6,
        //        RequireNonLetterOrDigit = false,
        //        RequireDigit = false,
        //        RequireLowercase = false,
        //        RequireUppercase = false,
        //    };

        //    // Configure user lockout defaults
        //    manager.UserLockoutEnabledByDefault = false;
        //    manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //    manager.MaxFailedAccessAttemptsBeforeLockout = 5;

        //    // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
        //    // You can write your own provider and plug it in here.
        //    //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
        //    //{
        //    //    MessageFormat = "Your security code is {0}"
        //    //});
        //    //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
        //    //{
        //    //    Subject = "Security Code",
        //    //    BodyFormat = "Your security code is {0}"
        //    //});
        //    //manager.EmailService = new EmailService();
        //    //manager.SmsService = new SmsService();

        //    var dataProtectionProvider = options.DataProtectionProvider;
        //    if (dataProtectionProvider != null)
        //    {
        //        manager.UserTokenProvider =
        //            new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
        //    }
        //    return manager;
        //}

        public async Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newPassword)
        {
            var result = await ChangePasswordAsync(accountId.ToString(), oldPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ChangeEmailAsync(Guid userId, string newEmail)
        {
            var result = await SetEmailAsync(userId.ToString(), newEmail);
            return result.Succeeded;
        }

        public async Task<Guid?> CreateUserAsync(string email, string password)
        {
            var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                };
            var result = await CreateAsync(user, password);
            if (result.Succeeded)
            {
                return Guid.Parse(user.Id);
            }
            return null;
        }


        public async Task<Guid> GetUserIdAsync(string email)
        {
            var user = await FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return Guid.Parse(user.Id);
        }
    }
}
