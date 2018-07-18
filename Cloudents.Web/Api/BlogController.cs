using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Blog")]
    public class BlogController : Controller
    {
        private readonly IReadRepositoryAsync<UniversityDto, long> _repository;

        public BlogController(IReadRepositoryAsync<UniversityDto, long> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(long id, CancellationToken token)
        {
            var result = await _repository.GetAsync(id, token).ConfigureAwait(false);
            return Json(result);
        }
    }
}