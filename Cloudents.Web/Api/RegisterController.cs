using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Web.Controllers;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using SbSignInManager = Cloudents.Web.Identity.SbSignInManager;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SbSignInManager _signInManager;

        private readonly IQueueProvider _queueProvider;
        private readonly IStringLocalizer<RegisterController> _localizer;
        private readonly IStringLocalizer<LogInController> _loginLocalizer;
        private readonly ILogger _logger;
        private readonly ICommandBus _commandBus;
        private readonly ICountryService _countryProvider;

        private const string Email = "email2";
        private const string EmailTime = "EmailTime";

        public RegisterController(UserManager<User> userManager, SbSignInManager signInManager,
             IQueueProvider queueProvider, IStringLocalizer<RegisterController> localizer, IStringLocalizer<LogInController> loginLocalizer, ILogger logger, ICountryService countryProvider, ICommandBus commandBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _localizer = localizer;
            _loginLocalizer = loginLocalizer;
            _logger = logger;
            _countryProvider = countryProvider;
            _commandBus = commandBus;
        }

        [HttpPost, ValidateRecaptcha("6LfyBqwUAAAAALL7JiC0-0W_uWX1OZvBY4QS_OfL"), ValidateEmail]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> PostAsync(
            [FromBody] RegisterRequest model,
            [FromHeader(Name = "User-Agent")] string? userAgent,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ModelState.AddModelError(nameof(model.Email), _localizer["UserExists"]);
                return BadRequest(ModelState);
            }

            var countryCode = await _countryProvider.GetUserCountryAsync(token);
            user = new User(model.Email, model.FirstName, model.LastName,
                CultureInfo.CurrentCulture, countryCode, model.UserType == UserType.Tutor);
            var p = await _userManager.CreateAsync(user, model.Password);

            if (p.Succeeded)
            {
                var t1 =  FinishRegistrationAsync(user, userAgent, token);
                //var command2 = new AddUserLocationCommand(user, HttpContext.GetIpAddress(), userAgent);
                var t2 = GenerateEmailAsync(user, token);
                //var t1 = _signInManager.SignInAsync(user,false);
                //var t3 = _commandBus.DispatchAsync(command2, token);
                await Task.WhenAll(t1, t2);
                return ReturnSignUserResponse.SignIn();
            }


            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }


        //private async Task<ReturnSignUserResponse> MakeDecisionAsync(User user,
        //    bool isExternal, string? password,
        //    CancellationToken token)
        //{

        //    if (user.PhoneNumber != null)
        //    {
        //        if (isExternal)
        //        {
        //            await _signInManager.SignInAsync(user, false);
        //            return ReturnSignUserResponse.SignIn();
        //        }
        //        throw new ArgumentException();
        //    }


        //    if (!user.EmailConfirmed)
        //    {
        //        await GenerateEmailAsync(user, token);
        //    }

        //    if (!isExternal)
        //    {
        //        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        //        if (!result.Succeeded)
        //        {
        //            throw new ArgumentException();
        //        }
        //        //passwod
        //    }
        //    await _signInManager.TempSignInAsync(user);
        //    return new ReturnSignUserResponse(RegistrationStep.RegisterSetPhone);
        //}


        [HttpPost("google")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> GoogleSignInAsync(
            [FromBody] GoogleTokenRequest model,
            [FromServices] IGoogleAuth service,
            [FromServices] IUserDirectoryBlobProvider blobProvider,
            [FromServices] TelemetryClient logClient,
            [FromServices] IHttpClientFactory clientFactory,
            [FromHeader(Name = "User-Agent")] string? userAgent,
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
                return ReturnSignUserResponse.SignIn();
                //return new ReturnSignUserResponse(false);
            }

            if (result2.IsLockedOut)
            {
                logClient.TrackTrace("user is locked out");
                ModelState.AddModelError("Google", _loginLocalizer["LockOut"]);
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(result.Email);

            //TODO: check what happens if google user is locked out
            if (result2.IsNotAllowed && user != null && await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("Google", _loginLocalizer["LockOut"]);
                return BadRequest(ModelState);
            }
            if (user == null)
            {
                var country = await _countryProvider.GetUserCountryAsync(cancellationToken);
                user = new User(result.Email,
                    result.FirstName, result.LastName,
                    result.Language, country, model.UserType == UserType.Tutor)
                {
                    EmailConfirmed = true
                };


                var result3 = await _userManager.CreateAsync(user);
                if (result3.Succeeded)
                {
                    if (!string.IsNullOrEmpty(result.Picture))
                    {
                        using var httpClient = clientFactory.CreateClient();
                        var message = await httpClient.GetAsync(result.Picture, cancellationToken);
                        await using var sr = await message.Content.ReadAsStreamAsync();
                        var mimeType = message.Content.Headers.ContentType;
                        try
                        {
                            var uri = await blobProvider.UploadImageAsync(user.Id, result.Picture, sr,
                                mimeType.ToString(), cancellationToken);
                            var fileName = uri.AbsolutePath.Split('/').Last();
                            user.UpdateUserImage(fileName);
                        }
                        catch (ArgumentException e)
                        {
                            logClient.TrackException(e, new Dictionary<string, string>()
                            {
                                ["FromGoogle"] = result.Picture
                            });
                        }
                    }
                    var t1 =  FinishRegistrationAsync(user, userAgent, cancellationToken);
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                    return ReturnSignUserResponse.SignIn();
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
            return ReturnSignUserResponse.SignIn();
        }




        private async Task GenerateEmailAsync(User user, CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = System.Net.WebUtility.UrlEncode(code);

            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var link = Url.Link("ConfirmEmail", new
            {
                user.Id,
                code = encodedCode,
                referral = TempData[HomeController.Referral]
            });
            _logger.Info("generate Email", new Dictionary<string, string>()
            {
                ["userId"] = user.Id.ToString(),
                ["code"] = code,
                ["encoded"] = encodedCode,
                ["link"] = link
            });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link), CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
        }

        private async Task FinishRegistrationAsync(User user,
            string userAgent, CancellationToken token)
        {
            if (TempData[HomeController.Referral] != null)
            {
                if (Base62.TryParse(TempData[HomeController.Referral]?.ToString(), out var base62))
                {
                    try
                    {
                        var command = new ReferringUserCommand(base62, user.Id);
                        await _commandBus.DispatchAsync(command, token);
                    }
                    catch (UserLockoutException)
                    {
                        _logger.Warning($"{user.Id} got locked referring user {TempData[HomeController.Referral]}");
                    }
                }
                else
                {
                    _logger.Error($"{user.Id} got wrong referring user {TempData[HomeController.Referral]}");
                }
                TempData.Remove(HomeController.Referral);
            }
            TempData.Clear();

            var command2 = new AddUserLocationCommand(user, HttpContext.GetIpAddress(), userAgent);
            var t1 = _commandBus.DispatchAsync(command2, token);
            var t2 = _signInManager.SignInAsync(user, false);
            await Task.WhenAll(t1, t2);
           
        }

        [Authorize]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> TutorVerifyEmail(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            await GenerateEmailAsync(user, token);
            return Ok();
        }

        [HttpPost("resend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> ResendEmailAsync(
            CancellationToken token)
        {
            var data = TempData.Peek(EmailTime);
            if (data != null)
            {
                var temp = DateTime.Parse(data.ToString()!, CultureInfo.InvariantCulture);

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
            await GenerateEmailAsync(user, token);
            return Ok();
        }
    }
}