using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;

        public ProfileController(IQueryBus queryBus, UserManager<User> userManager)
        {
            _queryBus = queryBus;
            _userManager = userManager;
        }

        // GET
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        
        public async Task<ActionResult<ProfileDto>> GetAsync(long id, CancellationToken token)
        {
            var query = new UserDataByIdQuery(id);
            var retVal = await _queryBus.QueryAsync<ProfileDto>(query, token).ConfigureAwait(false);
            if (retVal == null)
            {
                return NotFound();
            }
            return retVal;
        }

        /// <summary>
        /// Perform course search per user
        /// </summary>
        /// <param name="token"></param>
        /// <returns>list of courses for a user</returns>
        [Route("courses")]
        [HttpGet]
        public async Task<IEnumerable<CourseDto>> Get(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new CoursesQuery(userId);
            var t = await _queryBus.QueryAsync<IEnumerable<CourseDto>>(query, token);
            return t;
        }

        [Route("University")]
        [HttpGet]
        public async Task<IEnumerable<UniversityDto>> Get([FromServices] IQueryBus _queryBus, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UniversityQuery(userId);
            var t = await _queryBus.QueryAsync<IEnumerable<UniversityDto>>(query, token);
            return t;
        }
    }
}