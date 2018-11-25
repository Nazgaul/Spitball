﻿using System;
using System.Linq;
using System.Security.Claims;
using Cloudents.Core.Entities.Db;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Services
{
    public class UserIdInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        // private readonly ILogger _signInManager;
        private readonly IDataProtectionProvider _dataProtectProvider;

        public UserIdInitializer(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager,IDataProtectionProvider dataProtect)

        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _dataProtectProvider = dataProtect;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry)
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
                    telemetry.Context.User.Id = userId;
                    telemetry.Context.Session.Id = userId;
                    return;
                }

                // Get the encrypted cookie value
                //var opt = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
                var cookie = _httpContextAccessor.HttpContext.Request.Cookies[IdentityConstants.TwoFactorUserIdScheme];

                // Decrypt if found
                if (!string.IsNullOrEmpty(cookie))
                {

                    try
                    {
                        //https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.Cookies/PostConfigureCookieAuthenticationOptions.cs
                        var dataProtector = _dataProtectProvider.CreateProtector(
                            "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                            IdentityConstants.TwoFactorUserIdScheme, "v2");

                        var ticketDataFormat = new TicketDataFormat(dataProtector);
                        var ticket = ticketDataFormat.Unprotect(cookie);
                        var val = ticket.Principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name);
                        if (val != null)
                        {
                            telemetry.Context.User.Id = val.Value;
                            telemetry.Context.Session.Id = val.Value;
                        }
                    }
                    catch(Exception ex) 
                    {
                        
                    }

                    // return ticket.Principal.Claims;
                }
                //return null;
                //var z = _httpContextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme).Result;

                //if (z.Succeeded)
                //{

                //}

                //var val = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name);
                //if (val != null)
                //{
                //    telemetry.Context.User.Id = val.Value;
                //    telemetry.Context.Session.Id = val.Value;
                //}


                //if (_userManager.Get)
            }
        }
    }
}