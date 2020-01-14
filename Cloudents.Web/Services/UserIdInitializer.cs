using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;

namespace Cloudents.Web.Services
{
    public class UserIdInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdInitializer(
            IHttpContextAccessor httpContextAccessor
            )

        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry)
            {


                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    var principal = _httpContextAccessor.HttpContext.User;
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier);
                    telemetry.Context.User.Id = userId.Value;
                    return;
                }

                // Get the encrypted cookie value
                var cookie = _httpContextAccessor.HttpContext.Request.Cookies[IdentityConstants.TwoFactorUserIdScheme];
                // Decrypt if found
                if (!string.IsNullOrEmpty(cookie))
                {

                    try
                    {
                        if (_httpContextAccessor.HttpContext.RequestServices == null) return;
                        var dataProtector = _httpContextAccessor.HttpContext.RequestServices.GetDataProtector(
                            "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                            IdentityConstants.TwoFactorUserIdScheme, "v2");
                        ////https://github.com/aspnet/Security/blob/master/src/Microsoft.AspNetCore.Authentication.Cookies/PostConfigureCookieAuthenticationOptions.cs
                        //var dataProtector = _dataProtectProvider.CreateProtector(
                        //    "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                        //    IdentityConstants.TwoFactorUserIdScheme, "v2");

                        var ticketDataFormat = new TicketDataFormat(dataProtector);
                        var ticket = ticketDataFormat.Unprotect(cookie);
                        var val = ticket?.Principal?.Claims?.FirstOrDefault(f => f.Type == ClaimTypes.Name);
                        if (val != null)
                        {
                            telemetry.Context.User.Id = val.Value;
                            //telemetry.Context.Session.Id = val.Value;
                        }
                    }
                    catch (Exception)
                    {

                        //_logger.Value.TrackException(ex);
                        // ignored
                    }

                }

            }
        }
    }
}