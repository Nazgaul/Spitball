using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Services;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Cloudents.Web.Filters
{
    public sealed class SignInWithTokenAttribute : TypeFilterAttribute
    {
        public SignInWithTokenAttribute() : base(typeof(SignInToSystemImpl))
        {
        }
        public class SignInToSystemImpl : ActionFilterAttribute
        {
            public const string TokenQueryParam = "token";
            private readonly SignInManager<User> _signInManager;
            private readonly UserManager<User> _userManager;
            private readonly IDataProtect _dataProtect;
            private readonly ILogger _logger;

            public SignInToSystemImpl(SignInManager<User> signInManager, UserManager<User> userManager,
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
                var token = context.HttpContext.Request.Query[TokenQueryParam].ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    var claimUser = context.HttpContext.User;
                    if (_signInManager.IsSignedIn(claimUser))
                    {
                        context.Result = new RedirectResult(GetUrlWithoutTokenQuery(context));
                        return;
                    }

                    Enum.TryParse(context.HttpContext.Request.Query["channel"].ToString(), out CommunicationChannel communicationChannel);

                    try
                    {
                        var userId = _dataProtect.Unprotect(token);
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user == null)
                        {
                            context.Result = new RedirectResult(GetUrlWithoutTokenQuery(context));
                            return;
                        }

                        switch (communicationChannel)
                        {
                            case CommunicationChannel.None:
                                break;
                            case CommunicationChannel.Phone:
                                user.PhoneNumberConfirmed = true;

                                break;
                            case CommunicationChannel.Email:
                                user.EmailConfirmed = true;
                                break;
                        }

                        if (user.SecurityStamp == null)
                        {
                            await _userManager.UpdateSecurityStampAsync(user);
                            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                            code = UrlEncoder.Default.Encode(code);

                            if (context.Controller is Controller controller)
                            {
                                var link = controller.Url.Link("ResetPassword", new { user.Id, code, returnUrl = GetUrlWithoutTokenQuery(context) });
                                context.Result = new RedirectResult(link);
                                return;
                            }
                            //Need to change user to enter password
                        }
                        await _userManager.UpdateAsync(user);
                        await _signInManager.SignInAsync(user, false);

                    }
                    catch (CryptographicException ex)
                    {
                        //We just log the exception. user open the email too later and we can't sign it.
                        //If we see this persist then maybe we need to increase the amount of time
                        _logger.Exception(ex);
                    }
                    //var result = await SignInUserAsync(token,c);



                    //var link = Url.Link("ResetPassword", new { user.Id, code });
                    var url = GetUrlWithoutTokenQuery(context);

                    context.Result = new RedirectResult(url);
                    return;

                }
                await base.OnActionExecutionAsync(context, next);
            }

            private static string GetUrlWithoutTokenQuery(ActionExecutingContext context)
            {
                var queryStringDictionary = context.HttpContext.Request.Query.Where(w =>
                        !w.Key.Equals(TokenQueryParam, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(t => t.Key, t => t.Value.ToString());

                queryStringDictionary.Remove(TokenQueryParam);

                var url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(
                    context.HttpContext.Request.GetUri().AbsolutePath, queryStringDictionary);
                return url;
            }

            //private async Task<string> SignInUserAsync(string code, CommunicationChannel communicationChannel)
            //{
            //    try
            //    {

            //        var userId = _dataProtect.Unprotect(code);
            //        var user = await _userManager.FindByIdAsync(userId);
            //        if (user == null)
            //        {
            //            return false;
            //        }



            //        switch (communicationChannel)
            //        {
            //            case CommunicationChannel.None:
            //                break;
            //            case CommunicationChannel.Phone:
            //                user.PhoneNumberConfirmed = true;
            //                await _userManager.UpdateAsync(user);
            //                break;
            //            case CommunicationChannel.Email:
            //                user.EmailConfirmed = true;
            //                await _userManager.UpdateAsync(user);
            //                break;
            //        }

            //        if (user.PasswordHash == null && !_userManager.SupportsUserLogin)
            //        {
            //            var code2 = await _userManager.GeneratePasswordResetTokenAsync(user);
            //            code2 = UrlEncoder.Default.Encode(code);

            //            //Need to change user to enter password
            //        }

            //        //TODO: we need to take care verified the phone number /or email
            //        //TODO: need to redirect to create password page.
            //        //ViewBag.Auth = true;

            //        await _signInManager.SignInAsync(user, false);
            //        return true;
            //    }
            //    catch (CryptographicException ex)
            //    {
            //        //We just log the exception. user open the email too later and we can't sign it.
            //        //If we see this persist then maybe we need to increase the amount of time
            //        _logger.Exception(ex);
            //    }
            //    return false;
            //}
        }

    }
}