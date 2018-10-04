using System.Linq;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
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
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [HttpGet]
        public async Task<UniversityResponse> GetAsync([FromQuery] UniversityRequest model, CancellationToken token)
        {
            var countryClaim = User.Claims.FirstOrDefault(f => f.Type == AppClaimsPrincipalFactory.Country);
            var result = await _universityProvider.SearchAsync(model.Term,
                countryClaim?.Value, token).ConfigureAwait(false);
            return new UniversityResponse(result);
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
            var user = await _userManager.GetUserAsync(User);
            var command = new CreateUniversityCommand(model.Name, user.Id);
            await _commandBus.DispatchAsync(command, token);
            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            var command = new AssignUniversityToUserCommand(user.Id, model.UniversityId);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }

    }
}
