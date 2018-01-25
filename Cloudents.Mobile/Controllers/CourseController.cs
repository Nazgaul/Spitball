using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Extensions;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Course api controller
    /// </summary>
    [MobileAppController]
    public class CourseController : ApiController
    {
        private readonly ICourseSearch _courseProvider;
        private readonly IRepository<Course> _repository;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="courseProvider"></param>
        /// <param name="repository"></param>
        public CourseController(ICourseSearch courseProvider, IRepository<Course> repository)
        {
            _courseProvider = courseProvider;
            _repository = repository;
        }

        /// <summary>
        /// Perform course search
        /// </summary>
        /// <param name="model">params</param>
        /// <param name="token"></param>
        /// <returns>list of courses filter by input</returns>
        /// <exception cref="ArgumentException">university is empty</exception>
        [Route("api/course/search")]
        [HttpGet]
        public async Task<IHttpActionResult> Get([FromUri]  CourseRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetError());
            }

            var result = await _courseProvider.SearchAsync(model.Term, model.UniversityId.GetValueOrDefault(), token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Create academic course - note talk to RAM before applying
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token">Cancellation token</param>
        /// <returns>The id of the course created</returns>
        [Route("api/course/create")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateAcademicBoxAsync(CreateCourseRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid || !model.University.HasValue)
            {
                return BadRequest();
            }

            var course = new Course(model.CourseName, model.University.Value);

            var result = await _repository.AddAsync(course, token).ConfigureAwait(false);
            return Ok(result.Id);
        }
    }
}
