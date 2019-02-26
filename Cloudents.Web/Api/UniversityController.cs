using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Binders;

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
        private readonly UserManager<RegularUser> _userManager;
        private readonly SignInManager<RegularUser> _signInManager;
        private readonly ICommandBus _commandBus;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="universityProvider"></param>
        /// <param name="userManager"></param>
        /// <param name="commandBus"></param>
        /// <param name="signInManager"></param>
        public UniversityController(IUniversitySearch universityProvider, UserManager<RegularUser> userManager, ICommandBus commandBus, SignInManager<RegularUser> signInManager)
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
        /// <param name="profile">Not taken from the api</param>
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [HttpGet, AllowAnonymous]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { nameof(UniversityRequest.Term) })]

        public async Task<UniversitySearchDto> GetAsync([FromQuery] UniversityRequest model,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var result = await _universityProvider.SearchAsync(model.Term,
                profile.Country, token);
            return result;
        }

        

        [HttpPost("assign")]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new AssignUniversityToUserCommand(userId, model.Name, model.Country);
            await _commandBus.DispatchAsync(command, token);
            var user = await _userManager.GetUserAsync(User);

            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }




    }
}
