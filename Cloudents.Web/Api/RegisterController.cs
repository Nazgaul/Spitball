using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Cloudents.Command.Command;
using Cloudents.Command;
using Microsoft.AspNetCore.DataProtection;
using SbSignInManager = Cloudents.Web.Identity.SbSignInManager;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;

        private readonly IQueueProvider _queueProvider;
        private readonly ISmsSender _client;
        private readonly IStringLocalizer<RegisterController> _localizer;
        private readonly IStringLocalizer<LogInController> _loginLocalizer;
        private readonly ILogger _logger;

        internal const string Email = "email2";
        private const string EmailTime = "EmailTime";

        public RegisterController(UserManager<User> userManager, SbSignInManager signInManager,
             IQueueProvider queueProvider, ISmsSender client, IStringLocalizer<RegisterController> localizer, IStringLocalizer<LogInController> loginLocalizer, ILogger logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _client = client;
            _localizer = localizer;
            _loginLocalizer = loginLocalizer;
            _logger = logger;
        }

        [HttpPost, ValidateRecaptcha("6LfyBqwUAAAAALL7JiC0-0W_uWX1OZvBY4QS_OfL")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> Post(
            [FromBody] RegisterRequest model,
            [CanBeNull] ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                try
                {
                    return await MakeDecision(user, false, returnUrl, token);
                }
                catch (ArgumentException)
                {
                }

                ModelState.AddModelError(nameof(model.Email), _localizer["UserExists"]);
                return BadRequest(ModelState);
            }
            user = new User(model.Email, CultureInfo.CurrentCulture);
            var p = await _userManager.CreateAsync(user, model.Password);
            if (p.Succeeded)
            {
                await GenerateEmailAsync(user, returnUrl, token);
                return new ReturnSignUserResponse(NextStep.EmailConfirmed, true);
            }
            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }


        private async Task<ReturnSignUserResponse> MakeDecision(User user,
            bool isExternal,
            [CanBeNull] ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
           
            if (user.PhoneNumberConfirmed)
            {
                if (isExternal)
                {
                    await _signInManager.SignInAsync(user, false);
                    return new ReturnSignUserResponse(false);
                }

                throw new ArgumentException();
            }

            if (user.PhoneNumber != null)
            {
                var t1 = _signInManager.TempSignIn(user);
                var t2 = _client.SendSmsAsync(user, token);

                await Task.WhenAll(t1, t2);
                return new ReturnSignUserResponse(NextStep.VerifyPhone, true);
            }

            if (user.EmailConfirmed)
            {
                await _signInManager.TempSignIn(user);
                return new ReturnSignUserResponse(NextStep.EnterPhone, true);
            }

            await GenerateEmailAsync(user, returnUrl, token);
            return new ReturnSignUserResponse(NextStep.EmailConfirmed, true);
        }


        [HttpPost("google")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> GoogleSignInAsync([FromBody] GoogleTokenRequest model,
            [FromServices] IGoogleAuth service,
            [FromServices] IRestClient client,
            [FromServices] IUserDirectoryBlobProvider blobProvider,
            [FromServices] ICommandBus commandBus,
            [FromHeader(Name="user-agent")] string userAgent,
            [FromServices] TelemetryClient logClient,
            [FromServices]IDataProtectionProvider dataProtectProvider,
            CancellationToken cancellationToken)
        {
            var result = await service.LogInAsync(model.Token, cancellationToken);
            _logger.Info($"received google user {result}");
            if (result == null)
            {
                logClient.TrackTrace("result from google is null");
                ModelState.AddModelError("Google", _localizer["GoogleNoResponse"]);
                return BadRequest(ModelState);
            }

            var result2 = await _signInManager.ExternalLoginSignInAsync("Google", result.Id, true, true);
            if (result2.Succeeded)
            {
                // For india mobile - temp solution
                if (string.Equals(userAgent, "Spitball-Android", StringComparison.OrdinalIgnoreCase))
                {
                    var user2 = await _userManager.FindByEmailAsync(result.Email);
                    var dataProtector = dataProtectProvider.CreateProtector("Spitball").ToTimeLimitedDataProtector();
                    var code = dataProtector.Protect(user2.ToString(), DateTimeOffset.UtcNow.AddDays(5));

                    return Ok(new
                    {
                        code
                    });
                }

                return new ReturnSignUserResponse(false);
            }
           
            if (result2.IsLockedOut)
            {
                logClient.TrackTrace("user is locked out");
                ModelState.AddModelError("Google", _loginLocalizer["LockOut"]);
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(result.Email);
            if (result2.IsNotAllowed && await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("Google", _loginLocalizer["LockOut"]);
                return BadRequest(ModelState);
            }
            if (user == null)
            {
                user = new User(result.Email,
                    result.FirstName, result.LastName,
                    result.Language)
                {
                    EmailConfirmed = true
                };


                var result3 = await _userManager.CreateAsync(user);
                if (result3.Succeeded)
                {
                    if (!string.IsNullOrEmpty(result.Picture))
                    {
                        var (stream, _) = await client.DownloadStreamAsync(new Uri(result.Picture), cancellationToken);
                        try
                        {
                            var uri = await blobProvider.UploadImageAsync(user.Id, result.Picture, stream, token: cancellationToken);
                            var imageProperties = new ImageProperties(uri, ImageProperties.BlurEffect.None);
                            var url = Url.ImageUrl(imageProperties);
                            var fileName = uri.AbsolutePath.Split('/').LastOrDefault();
                            var command = new UpdateUserImageCommand(user.Id, url, fileName);
                            await commandBus.DispatchAsync(command, cancellationToken);

                        }
                        catch (ArgumentException e)
                        {
                            logClient.TrackException(e,new Dictionary<string, string>()
                            {
                                ["FromGoogle"] = result.Picture
                            });
                        }


                    }
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                    return await MakeDecision(user, true, null, cancellationToken);
                }
                logClient.TrackTrace($"failed to register {string.Join(", ", result3.Errors)}");

                ModelState.AddModelError("Google", _localizer["GoogleUserRegisteredWithEmail"]);
                return BadRequest(ModelState);
            }
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            
            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
            return await MakeDecision(user, true, null, cancellationToken);
        }




        private async Task GenerateEmailAsync(User user, [CanBeNull] ReturnUrlRequest returnUrl, CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
            var url = returnUrl?.Url;
            if (!Url.IsLocalUrl(url))
            {
                url = null;
            }
            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var link = Url.Link("ConfirmEmail", new { user.Id, code, returnUrl = url, referral = TempData[HomeController.Referral] });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link), CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
        }

        [HttpPost("resend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ResendEmailAsync(
            ReturnUrlRequest returnUrl,
            CancellationToken token)
        {
            var data = TempData.Peek(EmailTime);
            if (data != null)
            {
                var temp = DateTime.Parse(TempData.Peek(EmailTime).ToString(), CultureInfo.InvariantCulture);

                if (temp > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(0.5)))
                {
                    return Ok();
                }

            }

            var email = TempData.Peek(Email);
            if (email == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["EmailResend"]);
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(email.ToString());
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["UserNotExists"]);
                return BadRequest(ModelState);
            }

            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await GenerateEmailAsync(user, returnUrl, token);
            return Ok();
        }
    }
}