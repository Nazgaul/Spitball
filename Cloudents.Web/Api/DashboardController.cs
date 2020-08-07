using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using Cloudents.Core.Models;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Tutor")]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]

    public class DashboardController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IQueryBus _queryBus;

        public DashboardController(UserManager<User> userManager, IQueryBus queryBus)
        {
            _userManager = userManager;
            _queryBus = queryBus;
        }

        [HttpGet("actions")]
        public async Task<TutorActionsDto> GetTutorActionsAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new TutorActionsQuery(userId, profile.CountryRegion);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("notification")]
        public async Task<TutorNotificationDto> NotificationsAsync(
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new TutorNotificationQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("upcoming")]
        public async Task<IEnumerable<UpcomingStudyRoomDto>> FutureScheduleAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UpcomingLessonsQuery(userId);
            return await _queryBus.QueryAsync(query, token);

        }
    }
}
