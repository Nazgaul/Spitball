﻿using Cloudents.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Identity
{
    public static class SpitballIdentityExtensions {

        public const int PasswordRequiredLength = 8;
        public static void AddSbIdentity(this IServiceCollection services)
        {
            services.AddScoped<SignInManager<User>, SbSignInManager>();
            services.AddScoped<UserManager<User>, SbUserManager>();

            //RoleStore
            services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.SignIn.RequireConfirmedPhoneNumber = true;
                    options.User.AllowedUserNameCharacters = null;

                    options.User.RequireUniqueEmail = true;

                    options.Password.RequiredLength = PasswordRequiredLength;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Lockout.MaxFailedAccessAttempts = 3;
                }).AddDefaultTokenProviders()
                .AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>()
                .AddUserManager<SbUserManager>()
                .AddSignInManager<SbSignInManager>();
        }
    }
}