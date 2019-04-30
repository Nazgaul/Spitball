using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cloudents.Web.Filters
{
    public sealed class SignInWithTokenAttribute : TypeFilterAttribute
    {
        public SignInWithTokenAttribute() : base(typeof(SignInToSystemImpl))
        {
        }
        private class SignInToSystemImpl : ActionFilterAttribute
        {
            private readonly SignInManager<RegularUser> _signInManager;
            private readonly UserManager<RegularUser> _userManager;
            private readonly IDataProtect _dataProtect;
            private readonly ILogger _logger;

            public SignInToSystemImpl(SignInManager<RegularUser> signInManager, UserManager<RegularUser> userManager,
                IDataProtect dataProtect, ILogger logger)
            {
                _signInManager = signInManager;
                _userManager = userManager;
                _dataProtect = dataProtect;
                _logger = logger;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                var user = context.HttpContext.User;
                if (_signInManager.IsSignedIn(user))
                {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }

                var token = context.HttpContext.Request.Query["token"].ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    var result = await SignInUserAsync(token);

                    if (result && context.Controller is Controller controller)
                    {
                        controller.ViewBag.Auth = true;
                    }


                    var queryStringDictionary = context.HttpContext.Request.Query.Where(w =>
                            !w.Key.Equals("token", StringComparison.OrdinalIgnoreCase))
                        .ToDictionary(t => t.Key, t => t.Value.ToString());

                    queryStringDictionary.Remove("token");

                    var url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
                        context.HttpContext.Request.GetUri().AbsolutePath, queryStringDictionary);
                    
                    context.Result = new RedirectResult(url);
                    return;

                }
                await base.OnActionExecutionAsync(context, next);
            }

            private async Task<bool> SignInUserAsync(string code)
            {
                try
                {

                    var userId = _dataProtect.Unprotect(code);
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        //ViewBag.Auth = true;
                        await _signInManager.SignInAsync(user, false);
                        return true;
                    }
                }
                catch (CryptographicException ex)
                {
                    //We just log the exception. user open the email too later and we can't sign it.
                    //If we see this persist then maybe we need to increase the amount of time
                    _logger.Exception(ex);
                }
                return false;
            }
        }
    }
}