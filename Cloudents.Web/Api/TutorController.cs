using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]", Name = "Tutor"), ApiController]
    public class TutorController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;


        public TutorController(IQueryBus queryBus, UserManager<RegularUser> userManager)
        {
            _queryBus = queryBus;
            _userManager = userManager;
        }


        /// <summary>
        /// Used for tutor tab
        /// </summary>
        /// <param name="term"></param>
        /// <param name="profile"></param>
        /// <param name="page"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<WebResponseWithFacet<TutorListDto>> GetAsync(
            [RequiredFromQuery]string term,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            int page,
            CancellationToken token)
        {
            //TutorListTabQuery
            var query = new TutorListTabSearchQuery(term, profile.Country, page);
            var result = await _queryBus.QueryAsync(query, token);
            return new WebResponseWithFacet<TutorListDto>
            {
                Result = result,
                NextPageLink = Url.RouteUrl("TutorSearch", new { page = ++page, term })
            };
        }


        [HttpGet("search", Name = "TutorSearch")]
        public async Task<WebResponseWithFacet<TutorListDto>> GetAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            int page, CancellationToken token)
        {
            //TutorListTabQuery
            var query = new TutorListTabQuery(profile.Country, page: page);
            var result = await _queryBus.QueryAsync(query, token);
            return new WebResponseWithFacet<TutorListDto>
            {
                Result = result,
                NextPageLink = Url.RouteUrl("TutorSearch", new { page = ++page })
            };
        }


        /// <summary>
        /// Return relevant tutors base on user courses
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<TutorListDto>> GetTutorsAsync(
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListQuery(userId);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }

        /// <summary>
        /// Return relevant tutors base on user course
        /// </summary>
        /// <param name="courseName">The course name</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<TutorListDto>> GetTutorsAsync([RequiredFromQuery] string courseName,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListByCourseQuery(courseName, userId);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }
    }
}
