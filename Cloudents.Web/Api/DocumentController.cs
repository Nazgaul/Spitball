using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Storage;
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
        private readonly IBlobProvider<DocumentContainer> _blobProvider;

        public DocumentController(IQueryBus queryBus,
             ICommandBus commandBus, UserManager<User> userManager, IBlobProvider<DocumentContainer> blobProvider)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _blobProvider = blobProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(long id, CancellationToken token)
        {
            var query = new DocumentById(id);
            var tModel = _queryBus.QueryAsync<DocumentDto>(query, token);
            //var tContent = firstTime.GetValueOrDefault() ?
            //    _documentSearch.Value.ItemContentAsync(id, token) : Task.FromResult<string>(null);

            var filesTask = _blobProvider.FilesInDirectoryAsync("preview-", query.Id.ToString(), token);
            //var filesTask = _blobProvider.FilesInDirectoryAsync($"{query.Id}", token);

            await Task.WhenAll(tModel,filesTask);

            var model = tModel.Result;
            var files = filesTask.Result;//.Where(w => Regex.IsMatch(w.AbsolutePath, @"\d\."));
            if (model == null)
            {
                return NotFound();
            }
            //var preview = _factoryProcessor.PreviewFactory(model.Blob);
            //var result = await preview.ConvertFileToWebsitePreviewAsync(0, token).ConfigureAwait(false);

            return Ok(
                new
                {
                    details = model,
                    preview = files
                });
        }

        [HttpPost]
        public async Task<ActionResult> CreateDocumentAsync([FromBody]CreateDocumentRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var command = new CreateDocumentCommand(model.BlobName, model.Name, model.Type,
                model.Course, model.Tags, userId, model.Professor);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}