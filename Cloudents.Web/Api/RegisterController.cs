using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
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
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Authorization;
using SbSignInManager = Cloudents.Web.Identity.SbSignInManager;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController]
    public class RegisterController : Controller
    {
        private readonly SbUserManager _userManager;
        private readonly SbSignInManager _signInManager;

        private readonly IQueueProvider _queueProvider;
        private readonly ISmsSender _smsSender;
        private readonly IStringLocalizer<RegisterController> _localizer;
        private readonly IStringLocalizer<LogInController> _loginLocalizer;
        private readonly ILogger _logger;
        private readonly ICountryService _countryProvider;

        internal const string Email = "email2";
        private const string EmailTime = "EmailTime";

        public RegisterController(SbUserManager userManager, SbSignInManager signInManager,
             IQueueProvider queueProvider, ISmsSender client, IStringLocalizer<RegisterController> localizer, IStringLocalizer<LogInController> loginLocalizer, ILogger logger, ICountryService countryProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _queueProvider = queueProvider;
            _smsSender = client;
            _localizer = localizer;
            _loginLocalizer = loginLocalizer;
            _logger = logger;
            _countryProvider = countryProvider;
        }

        [HttpPost, ValidateRecaptcha("6LfyBqwUAAAAALL7JiC0-0W_uWX1OZvBY4QS_OfL"), ValidateEmail]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ReturnSignUserResponse>> PostAsync(
            [FromBody] RegisterRequest model,
            //ReturnUrlRequest? returnUrl,
            [FromServices] ICountryProvider country,
            CancellationToken token)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                try
                {
                    return await MakeDecisionAsync(user, false,  token);
                }
                catch (ArgumentException)
                {
                }

                ModelState.AddModelError(nameof(model.Email), _localizer["UserExists"]);
                return BadRequest(ModelState);
            }

            var countryCode = await _countryProvider.GetUserCountryAsync(token);
            country.GetCallingCode(countryCode);
            user = new User(model.Email, model.FirstName, model.LastName, 
                CultureInfo.CurrentCulture, countryCode, model.Gender);
            var p = await _userManager.CreateAsync(user, model.Password);

            var retVal = await _userManager.SetPhoneNumberAndCountryAsync(user, model.PhoneNumber, countryCode, token);
            await _smsSender.SendSmsAsync(user, token);
            if (p.Succeeded && retVal.Succeeded)
            {
                //TODO
                await GenerateEmailAsync(user,  token);
                return Ok();// new ReturnSignUserResponse(RegistrationStep.RegisterEmailConfirmed);
            }


            ModelState.AddIdentityModelError(p);
            return BadRequest(ModelState);
        }


        private async Task<ReturnSignUserResponse> MakeDecisionAsync(User user,
            bool isExternal,
            CancellationToken token)
        {

            if (user.PhoneNumberConfirmed)
            {
                if (isExternal)
                {
                    await _signInManager.SignInAsync(user, false);
                    return ReturnSignUserResponse.SignIn();
                    // return new ReturnSignUserResponse(false);
                }

                throw new ArgumentException();
            }

            if (user.PhoneNumber != null)
            {
                var t1 = _signInManager.TempSignIn(user);
                var t2 = _smsSender.SendSmsAsync(user, token);

                await Task.WhenAll(t1, t2);
                return new ReturnSignUserResponse(RegistrationStep.RegisterVerifyPhone, new
                {
                    phoneNumber = user.PhoneNumber
                });
            }

            if (user.EmailConfirmed)
            {
                await _signInManager.TempSignIn(user);
                return new ReturnSignUserResponse(RegistrationStep.RegisterSetPhone);
            }
            await _signInManager.TempSignIn(user);
            await GenerateEmailAsync(user,  token);
            return new ReturnSignUserResponse(RegistrationStep.RegisterSetPhone);
        }


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
                    result.Language, country)
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
                            var imageProperties = new ImageProperties(uri, ImageProperties.BlurEffect.None);
                            var url = Url.ImageUrl(imageProperties);
                            var fileName = uri.AbsolutePath.Split('/').LastOrDefault();
                            user.UpdateUserImage(url, fileName);
                        }
                        catch (ArgumentException e)
                        {
                            logClient.TrackException(e, new Dictionary<string, string>()
                            {
                                ["FromGoogle"] = result.Picture
                            });
                        }
                    }
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", result.Id, result.Name));
                    return await MakeDecisionAsync(user, true,  cancellationToken);
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
            return await MakeDecisionAsync(user, true,  cancellationToken);
        }




        private async Task GenerateEmailAsync(User user,  CancellationToken token)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = UrlEncoder.Default.Encode(code);
          
            TempData[EmailTime] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var link = Url.Link("ConfirmEmail", new { user.Id, code,  referral = TempData[HomeController.Referral] });
            TempData[Email] = user.Email;
            var message = new RegistrationEmail(user.Email, HtmlEncoder.Default.Encode(link), CultureInfo.CurrentUICulture);
            await _queueProvider.InsertMessageAsync(message, token);
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
            await GenerateEmailAsync(user,  token);
            return Ok();
        }

        [HttpPost("userType"), Authorize]
        public async Task<IActionResult> SetUserTypeAsync([FromBody] SetUserTypeRequest model,
            [FromServices] ICommandBus commandBus, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new SetUserTypeCommand(userId, model.UserType);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost("childName"), Authorize]
        public async Task<IActionResult> SetChildNameAsync([FromBody] SetChildNameRequest model,
            [FromServices] ICommandBus commandBus, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new SetChildNameCommand(userId, model.Name, model.Grade);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("grade")]
        public async Task<IActionResult> SetUserGradeAsync([FromBody] UserGradeRequest model,
            [FromServices] ICommandBus commandBus,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new SetUserGradeCommand(userId, model.Grade);
            await commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}