using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Courses;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly IUrlBuilder _urlBuilder;

        public CourseController(IQueryBus queryBus, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _urlBuilder = urlBuilder;
        }

        [Route("course/{id:long}", Name = SeoTypeString.Course)]

        public async Task<IActionResult> Index([FromRoute] long id, CancellationToken token)
        {
           
            var query = new CourseByIdQuery(id, 0);
            var result = await _queryBus.QueryAsync(query, token);
            if (result == null)
            {
                return NotFound();
            }
            ViewBag.ogImage = _urlBuilder.BuildCourseThumbnailEndPoint(result.Id, result.Version);
            ViewBag.title =$"{result.Name}";
            ViewBag.metaDescription = result.Description;
            return View("Index");
        }
    }
}
