using System;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Net.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Microsoft.AspNetCore.Hosting;
using Cloudents.Infrastructure;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class TutorController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly UserManager<RegularUser> _userManager;
        private readonly IMondayProvider _mondayProvider;


        public TutorController(IQueryBus queryBus, UserManager<RegularUser> userManager,
            IMondayProvider mondayProvider)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _mondayProvider = mondayProvider;
        }


        /// <summary>
        /// Used for tutor tab
        /// </summary>
        /// <param name="term"></param>
        /// <param name="profile"></param>
        /// <param name="page"></param>
        /// <param name="tutorSearch"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("search", Name = "TutorSearch")]
        public async Task<WebResponseWithFacet<TutorListDto>> GetAsync(
            string term,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            int page,
            [FromServices] ITutorSearch tutorSearch,

            CancellationToken token)
        {
            //TODO make it better
            if (string.IsNullOrEmpty(term))
            {
                var query = new TutorListTabQuery(profile.Country, page: page);
                var result = await _queryBus.QueryAsync(query, token);
                return new WebResponseWithFacet<TutorListDto>
                {
                    Result = result,
                    NextPageLink = Url.RouteUrl("TutorSearch", new { page = ++page })
                };
            }
            else
            {
                var query = new TutorListTabSearchQuery(term, profile.Country, page);
                var result = await tutorSearch.SearchAsync(query, token);
                return new WebResponseWithFacet<TutorListDto>
                {
                    Result = result,
                    NextPageLink = Url.RouteUrl("TutorSearch", new {page = ++page, term})
                };
            }
        }


       
       


        /// <summary>
        /// Return relevant tutors base on user courses -on all courses tab - feed
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<TutorListDto>> GetTutorsAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListQuery(userId, profile.Country);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }

        /// <summary>
        /// Return relevant tutors base on user course - on specific course tab - feed
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
        [HttpPost("request")]
        public async Task<IActionResult> RequestTutorAsync(RequestTutorRequest model,
            [FromServices]  IQueueProvider queueProvider,
            [FromServices] IHostingEnvironment configuration,
            [FromHeader(Name = "referer")] Uri referer,
            CancellationToken token)
        {
            //RequestTutorEmail
            if ( _userManager.TryGetLongUserId(User,out var userId))
            {
                var query = new UserEmailInfoQuery(userId);
                var userInfo = await _queryBus.QueryAsync(query, token);
                model.Phone = userInfo.PhoneNumber;
                model.Name = userInfo.Name;
                model.Email = userInfo.Email;
                model.University = userInfo.University;

            }
            else
            {
                //TODO : need to register user
            }
            // var query = new UserEmailInfoQuery(userId);
            //var userInfo = await _queryBus.QueryAsync(query, token);

            var email = new RequestTutorEmail();
            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                email.Dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(model).ToString());
            }
           
            var utmSource = referer.ParseQueryString()["utm_source"];
            var task1 = queueProvider.InsertMessageAsync(email, token);
            var task2 = _mondayProvider.CreateRecordAsync(new MondayMessage(model.Course,
                configuration.IsProduction(),
                model.Name,
                model.Phone,
                model.Text,
                model.University,
                utmSource
                ), token);


            await Task.WhenAll(task1, task2);
            return Ok();
        }

        //[HttpPost("anonymousRequest")]
        //public async Task<IActionResult> AnonymousRequestTutorAsync(AnonymousRequestTutorRequest model,
        //    [FromServices]  IQueueProvider queueProvider,
        //    [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
        //    [FromServices] IHostingEnvironment configuration,
        //    [FromHeader(Name = "referer")] Uri referer,
        //    CancellationToken token)
        //{
        //    var email = new RequestTutorEmail()
        //    {
        //        Country = profile.Country,
        //        PhoneNumber = model.PhoneNumber,
        //        Text = model.Text,
        //        Email = model.Email,
        //        Name = model.Name,
        //        Referer = referer.AbsoluteUri,
        //        IsProduction = configuration.IsProduction()
        //    };
           
        //    var task1 = queueProvider.InsertMessageAsync(email, token);
        //    var task2 = _mondayProvider.CreateRecordAsync(email, token);

            
        //    await Task.WhenAll(task1, task2);
        //    return Ok();
        //}

        

        [HttpGet("reviews")]
        public async Task<IEnumerable<AboutTutorDto>> GetReviwesAsync(CancellationToken token)
        {
            var query = new AboutTutorQuery();
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }
    }
}
