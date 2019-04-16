﻿using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Wangkanai.Detection;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Framework;
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


        //[HttpGet]
        //public WebResponseWithFacet<TutorDto> GetAsync(CancellationToken token)
        //{


        //    return new WebResponseWithFacet<TutorDto>
        //    {
        //        Result = new TutorDto[0],
        //        //Sort = EnumExtension.GetValues<TutorRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
        //        //Filters = new IFilters[]
        //        //{
        //        //    new Filters<string>(nameof(TutorRequest.Filter),_localizer["StatusFilter"],
        //        //        EnumExtension.GetValues<TutorRequestFilter>()
        //        //            .Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization())))
        //        //},
        //        NextPageLink = null
        //    };
        //}

        //public class TutorDto
        //{

        //}

        [HttpGet]
        public async Task<IEnumerable<TutorListDto>> GetTutorsAsync([RequiredFromQuery] 
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User,out var userId);
            var query = new TutorListQuery(userId);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }

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
