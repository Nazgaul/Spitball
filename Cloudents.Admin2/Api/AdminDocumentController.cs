﻿using Cloudents.Admin2.Models;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Extension;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                var file = await _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token).FirstOrDefaultAsync(token);

                if (file != null)
                {

                    document.Preview =
                      await blobProvider.GeneratePreviewLinkAsync(file,
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

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery(Name = "id"), MaxLength(200)] IEnumerable<long> ids, CancellationToken token)
        {
            foreach (var id in ids)
            {
                var command = new DeleteDocumentCommand(id);
                await _commandBus.DispatchAsync(command, token);
            }
            return Ok();
        }

        //[HttpPost("unDelete")]
        //[Authorize]
        //public async Task<IActionResult> UnDelete([FromBody] UnDeleteDocumentRequest model, CancellationToken token)
        //{
        //    var command = new UnDeleteDocumentCommand(model.Id);
        //    await _commandBus.DispatchAsync(command, token);
        //    return Ok();
        //}

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

            var query = new FlaggedDocumentQuery(User.GetSbCountryClaim());
            var retVal = await _queryBus.QueryAsync(query, token);
            var tasks = new Lazy<List<Task>>();

            foreach (var document in retVal)
            {

                var file = await _blobProvider.FilesInDirectoryAsync("preview-0", document.Id.ToString(), token).FirstOrDefaultAsync(token);

                if (file != null)
                {
                    document.Preview =
                     await blobProvider.GeneratePreviewLinkAsync(file,
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
