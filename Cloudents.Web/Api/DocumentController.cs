using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly Lazy<IDocumentSearch> _documentSearch;
        private readonly IFactoryProcessor _factoryProcessor;

        public DocumentController(IQueryBus queryBus,
            Lazy<IDocumentSearch> documentSearch,
            IFactoryProcessor factoryProcessor, ICommandBus commandBus, UserManager<User> userManager)
        {
            _queryBus = queryBus;
            _documentSearch = documentSearch;
            _factoryProcessor = factoryProcessor;
            _commandBus = commandBus;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(long id, bool? firstTime, CancellationToken token)
        {
            var query = new DocumentById(id);
            var tModel = _queryBus.QueryAsync<DocumentDto>(query, token);
            var tContent = firstTime.GetValueOrDefault() ?
                _documentSearch.Value.ItemContentAsync(id, token) : Task.FromResult<string>(null);
            await Task.WhenAll(tModel, tContent).ConfigureAwait(false);
            var model = tModel.Result;
            if (model == null)
            {
                return NotFound();
            }
            var preview = _factoryProcessor.PreviewFactory(model.Blob);
            var result = await preview.ConvertFileToWebsitePreviewAsync(0, token).ConfigureAwait(false);
            
            return Ok(
                new
                {
                    details = model,
                    content = tContent.Result,
                    preview = result
                });
        }

        [HttpPost]
        public async Task<ActionResult> CreateDocumentAsync([FromBody]CreateDocumentRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var command = new CreateDocumentCommand(model.BlobName, model.Name, model.Type, model.Courses, model.Tags,
                userId);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}