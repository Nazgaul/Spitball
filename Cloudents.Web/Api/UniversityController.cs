//using System.Diagnostics.CodeAnalysis;
//using Cloudents.Command;
//using Cloudents.Command.Universities;
//using Cloudents.Core;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Exceptions;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Models;
//using Cloudents.Web.Binders;
//using Cloudents.Web.Extensions;
//using Cloudents.Web.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Web.Api
//{
//    /// <inheritdoc />
//    /// <summary>
//    /// University api controller
//    /// </summary>
//    [Route("api/[controller]"), ApiController, Authorize]
//    public class UniversityController : ControllerBase
//    {
//        private readonly IUniversitySearch _universityProvider;
//        private readonly UserManager<User> _userManager;
//        private readonly SignInManager<User> _signInManager;
//        private readonly ICommandBus _commandBus;

//        /// <inheritdoc />
//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="universityProvider"></param>
//        /// <param name="userManager"></param>
//        /// <param name="commandBus"></param>
//        /// <param name="signInManager"></param>
//        public UniversityController(IUniversitySearch universityProvider, UserManager<User> userManager, ICommandBus commandBus, SignInManager<User> signInManager)
//        {
//            _universityProvider = universityProvider;
//            _userManager = userManager;
//            _commandBus = commandBus;
//            _signInManager = signInManager;
//        }

//        /// <summary>
//        /// Get list of universities
//        /// </summary>
//        /// <param name="model">object of query string</param>
//        /// <param name="profile"></param>
//        /// <param name="token"></param>
//        /// <returns>list of universities</returns>
//        [HttpGet, AllowAnonymous]
//        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client,
//            VaryByQueryKeys = new[] { "*" })]

//        public async Task<UniversitySearchDto> GetAsync([FromQuery] UniversityRequest model,
//            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
//            CancellationToken token)
//        {
//            var result = await _universityProvider.SearchAsync(model.Term, model.Page,
//                profile.Country, token);
//            return result;
//        }



//        [HttpPost("set")]
//        [SuppressMessage("ReSharper", "PossibleInvalidOperationException", Justification = "Asp.net core validation will fix that")]
//        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
//        {
//            var userId = _userManager.GetLongUserId(User);
//            var command = new UserJoinUniversityCommand(userId, model.Id!.Value);
//            await _commandBus.DispatchAsync(command, token);
//            var user = await _userManager.GetUserAsync(User);
//            await _signInManager.RefreshSignInAsync(user);
//            return Ok();
//        }

//        [HttpPost("create")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status409Conflict)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesDefaultResponseType]
//        public async Task<IActionResult> CreateUniversityAsync([FromBody] CreateUniversityRequest model, CancellationToken token)
//        {

//            var userId = _userManager.GetLongUserId(User);
//            try
//            {
//                var command = new CreateUniversityCommand(userId, model.Name, model.Country);
//                await _commandBus.DispatchAsync(command, token);
//                var user = await _userManager.GetUserAsync(User);

//                await _signInManager.RefreshSignInAsync(user);
//                return Ok();
//            }
//            catch (DuplicateRowException)
//            {
//                return Conflict();
//            }
//        }




//    }
//}
