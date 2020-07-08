using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Storage;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class ExternalSignInController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly TelemetryClient _logClient;
        private readonly ICountryService _countryProvider;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUserDirectoryBlobProvider _blobProvider;

        public ExternalSignInController(SignInManager<User> signInManager, TelemetryClient logClient, UserManager<User> userManager, IHttpClientFactory clientFactory, IUserDirectoryBlobProvider blobProvider, ICountryService countryProvider)
        {
            _signInManager = signInManager;
            _logClient = logClient;
            _userManager = userManager;
            _clientFactory = clientFactory;
            _blobProvider = blobProvider;
            _countryProvider = countryProvider;
        }

        [Route("External/Google")]
        public IActionResult Index(string returnUrl, UserType userType)
        {
            var redirectUrl = Url.Action("ExternalCallBack",new
            {
                returnUrl,
                userType

            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            
            return new ChallengeResult("Google", properties);
            //return Ok("Hi Man");
        }

        [Route("ExternalLogin")]
        public async Task<IActionResult> ExternalCallBack(
            string? returnUrl,
            UserType? userType,
            string? remoteError,
            CancellationToken cancellationToken)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                _logClient.TrackTrace($"Error from external provider: {remoteError}");
                return Redirect("/");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
           
            if (info == null)
            {
                _logClient.TrackTrace("result from google is null");
                return RedirectToPage("/");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor : true);
            if (result.Succeeded)
            {
               // _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                //TODO
               // return RedirectToPage("./Lockout");
            }

            var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var country = await _countryProvider.GetUserCountryAsync(cancellationToken);
           // return LocalRedirect(returnUrl);

            ////TODO: check what happens if google user is locked out
          
            if (user == null)
            {
               
                user = new User(email,
                    firstName, lastName,
                    CultureInfo.CurrentCulture, country, userType == UserType.Tutor)
                {
                    EmailConfirmed = true
                };
                var result3 = await _userManager.CreateAsync(user);
                if (result3.Succeeded)
                {
                    var picture = info.Principal.FindFirstValue("image");
                    if (!string.IsNullOrEmpty(picture))
                    {
                        using var httpClient = _clientFactory.CreateClient();
                        var message = await httpClient.GetAsync(picture, cancellationToken);
                        await using var sr = await message.Content.ReadAsStreamAsync();
                        var mimeType = message.Content.Headers.ContentType;
                        try
                        {
                            var uri = await _blobProvider.UploadImageAsync(user.Id, picture, sr,
                                mimeType.ToString(), cancellationToken);
                            var fileName = uri.AbsolutePath.Split('/').Last();
                            user.UpdateUserImage(fileName);
                        }
                        catch (ArgumentException e)
                        {
                            _logClient.TrackException(e, new Dictionary<string, string>()
                            {
                                ["FromGoogle"] = picture
                            });
                        }
                    }
                    await _userManager.AddLoginAsync(user, info);
                    return LocalRedirect(returnUrl);
                }
                _logClient.TrackTrace($"failed to register {string.Join(", ", result3.Errors)}");

                return LocalRedirect(returnUrl);
            }
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            await _userManager.AddLoginAsync(user, info);
            return LocalRedirect(returnUrl);
        }
    }
}
