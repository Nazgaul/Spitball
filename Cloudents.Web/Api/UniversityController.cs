using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// University api controller
    /// </summary>
    [Route("api/[controller]"), ApiController, Authorize]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversitySearch _universityProvider;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ICommandBus _commandBus;


        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="universityProvider"></param>
        /// <param name="userManager"></param>
        /// <param name="commandBus"></param>
        /// <param name="signInManager"></param>
        public UniversityController(IUniversitySearch universityProvider, UserManager<User> userManager, ICommandBus commandBus, SignInManager<User> signInManager)
        {
            _universityProvider = universityProvider;
            _userManager = userManager;
            _commandBus = commandBus;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Get list of universities
        /// </summary>
        /// <param name="model">object of query string</param>
        /// <param name="country">Not taken from the api</param>
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [HttpGet]
        public async Task<UniversitySearchDto> GetAsync([FromQuery] UniversityRequest model,
            [Required(ErrorMessage = "NeedCountry"), ClaimModelBinder(AppClaimsPrincipalFactory.Country)] string country,
            CancellationToken token)
        {
            var result = await _universityProvider.SearchAsync(model.Term,
                country, token).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Create new university and assign the user to that university
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUniversityRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new CreateUniversityCommand(model.Name, userId);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);

            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new AssignUniversityToUserCommand(userId, model.UniversityId);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            var user = await _userManager.GetUserAsync(User);

            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }

    }
}
