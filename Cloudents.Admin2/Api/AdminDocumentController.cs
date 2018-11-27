using Cloudents.Admin2.Models;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly ICommandBus _commandBus;

        public AdminDocumentController(IQueryBus queryBus, IBlobProvider<DocumentContainer> blobProvider, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _blobProvider = blobProvider;
            _commandBus = commandBus;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<PendingDocumentDto>> Get(
            [FromServices] IBlobProvider blobProvider,
            CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<PendingDocumentDto>>(query, token);
            foreach (var id in retVal)
            {
                var files = await _blobProvider.FilesInDirectoryAsync("preview-", id.Id.ToString(), token);
                var file = files.FirstOrDefault();
                if (file != null)
                {
                    id.Preview =
                        blobProvider.GeneratePreviewLink(file,
                            20); // filesTask.Result.Select(s => blobProvider.GeneratePreviewLink(s, 20));
                }
            }
            return retVal.Where(w => w.Preview != null);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, CancellationToken token)
        {
            var command = new DeleteDocumentCommand(id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        [HttpPost]
        public void ApproveAsync(ApproveDocumentRequest model)
        {

        }
    }
}
