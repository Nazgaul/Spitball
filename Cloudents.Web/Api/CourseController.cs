using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Course")]
    public class CourseController : Controller
    {
        private readonly ICourseSearch m_CourseProvider;

        public CourseController(ICourseSearch courseProvider)
        {
            m_CourseProvider = courseProvider;
        }

        public async Task<IActionResult> Get(string term, long universityId, CancellationToken token)
        {
            if (universityId == default)
            {
                throw new ArgumentException(nameof(universityId));
            }
            var result = await m_CourseProvider.SearchAsync(term, universityId, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}