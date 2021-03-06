﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Web.Binders;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Api
{
    [Route("api/[controller]"), ApiController, Authorize]
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogProvider _blogProvider;

        public BlogController(IBlogProvider blogProvider)
        {
            _blogProvider = blogProvider;
        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<DashboardBlogDto>> Get(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            return await _blogProvider.GetBlogAsync(BlogQuery.Blog(profile.CountryRegion),  token);
        }

        [HttpGet("marketing")]
        public async Task<IEnumerable<DashboardBlogDto>> GetMarketingAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            return await _blogProvider.GetBlogAsync(BlogQuery.Marketing(profile.CountryRegion), token);
        }
    }
}
