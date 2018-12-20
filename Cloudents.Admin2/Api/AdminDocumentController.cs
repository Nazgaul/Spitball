﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Query.Admin;

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
        private readonly IQueueProvider _queueProvider;

        public AdminDocumentController(IQueryBus queryBus, IBlobProvider<DocumentContainer> blobProvider,
            ICommandBus commandBus, IQueueProvider queueProvider)
        {
            _queryBus = queryBus;
            _blobProvider = blobProvider;
            _commandBus = commandBus;
            _queueProvider = queueProvider;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<PendingDocumentDto>> Get(
            [FromServices] IBlobProvider blobProvider,
            CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<PendingDocumentDto>>(query, token);
            var tasks = new Lazy<List<Task>>();
            var counter = 0;
            foreach (var document in retVal)
            {

                var files = await  _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token);
                var file = files.FirstOrDefault();
                if (file != null)
                {
                    document.Preview =
                        blobProvider.GeneratePreviewLink(file,
                            20);

                    document.SiteLink = Url.RouteUrl("DocumentDownload", new {id = document.Id});
                    counter++;
                }
                else
                {

                    var t =  _queueProvider.InsertBlobReprocessAsync(document.Id);
                    tasks.Value.Add(t);
                }

                if (counter >= 21)
                {
                    break;
                }
            }

            if (tasks.IsValueCreated)
            {
                await Task.WhenAll(tasks.Value);
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
        public async Task<IActionResult> ApproveAsync([FromBody] ApproveDocumentRequest model, CancellationToken token)
        {
            var command = new ApproveDocumentCommand(model.Id);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpGet("flagged")]
        public async Task<IEnumerable<FlaggedDocumentDto>> FlagAsync([FromServices] IBlobProvider blobProvider, CancellationToken token)
        {
            var query = new AdminEmptyQuery();
            var retVal = await _queryBus.QueryAsync<IList<FlaggedDocumentDto>>(query, token);
            
            
            return retVal;
            
        }


        [HttpPost("unFlag")]
        public async Task<ActionResult> UnFlagAnswerAsync([FromBody] UnFlagDocumentRequest model, CancellationToken token)
        {
            var command = new UnFlagDocumentCommand(model.Id);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }
    }
}
