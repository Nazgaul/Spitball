using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Cloudents.Admin2.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminDocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly IDocumentDirectoryBlobProvider _blobProvider;
        private readonly ICommandBus _commandBus;
        private readonly IQueueProvider _queueProvider;

        public AdminDocumentController(IQueryBus queryBus, IDocumentDirectoryBlobProvider blobProvider,
            ICommandBus commandBus, IQueueProvider queueProvider)
        {
            _queryBus = queryBus;
            _blobProvider = blobProvider;
            _commandBus = commandBus;
            _queueProvider = queueProvider;
        }

        // GET: api/<controller>
        [HttpGet(Name = "Pending")]
        public async Task<PendingDocumentResponse> Get(long? fromId,
            [FromServices] IBlobProvider blobProvider,
            CancellationToken token)
        {

            var query = new PendingDocumentQuery(fromId, User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            var tasks = new Lazy<List<Task>>();
            var counter = 0;
            long? id = null;
            foreach (var document in retVal)
            {
                id = document.Id;
                var files = await _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token);
                var file = files.FirstOrDefault();
                if (file != null)
                {

                    document.Preview =
                        blobProvider.GeneratePreviewLink(file,
                            TimeSpan.FromMinutes(20));

                    counter++;
                }
                else
                {

                    var t = _queueProvider.InsertBlobReprocessAsync(document.Id);
                    tasks.Value.Add(t);
                }
                document.SiteLink = Url.RouteUrl("DocumentDownload", new { id = document.Id });

                if (counter >= 21)
                {
                    break;
                }
            }

            if (tasks.IsValueCreated)
            {
                await Task.WhenAll(tasks.Value);
            }

            string nextLink = null;
            if (id.HasValue)
            {
                nextLink = Url.RouteUrl("Pending", new
                {
                    fromId = id.Value
                });
            }

            return new PendingDocumentResponse()
            {
                Documents = retVal,
                NextLink = nextLink
            };
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(long id, CancellationToken token)
        {
            var command = new DeleteDocumentCommand(id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAsync([FromBody] ApproveDocumentRequest model, CancellationToken token)
        {
            var command = new ApproveDocumentCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("flagged")]
        public async Task<IEnumerable<FlaggedDocumentDto>> FlagAsync
            ([FromServices] IBlobProvider blobProvider, CancellationToken token)
        {

            var query = new FlaggedDocumentQuery(User.GetCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            var tasks = new Lazy<List<Task>>();

            foreach (var document in retVal)
            {

                var files = await _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token);
                var file = files.FirstOrDefault();
                if (file != null)
                {
                    document.Preview =
                        blobProvider.GeneratePreviewLink(file,
                            TimeSpan.FromMinutes(20));

                    document.SiteLink = Url.RouteUrl("DocumentDownload", new { id = document.Id });
                }
                else
                {

                    var t = _queueProvider.InsertBlobReprocessAsync(document.Id);
                    tasks.Value.Add(t);
                }


            }

            if (tasks.IsValueCreated)
            {
                await Task.WhenAll(tasks.Value);
            }

            //return retVal.Where(w => w.Preview != null);
            return retVal;

        }


        [HttpPost("unFlag")]
        public async Task<ActionResult> UnFlagAnswerAsync([FromBody] UnFlagDocumentRequest model, CancellationToken token)
        {
            var command = new UnFlagDocumentCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
