using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly Lazy<IDocumentSearch> _documentSearch;
        private readonly IFactoryProcessor _factoryProcessor;

        public DocumentController(IQueryBus queryBus,
            Lazy<IDocumentSearch> documentSearch,
            IFactoryProcessor factoryProcessor)
        {
            _queryBus = queryBus;
            _documentSearch = documentSearch;
            _factoryProcessor = factoryProcessor;
        }

        //TODO we need to fix that
        [HttpGet]
        public async Task<IActionResult> GetAsync(long id, bool? firstTime, CancellationToken token)
        {
            var query = new DocumentById(id);
            var tModel = _queryBus.QueryAsync<DocumentDto>(query, token);
            var tContent = firstTime.GetValueOrDefault() ?
                _documentSearch.Value.ItemContentAsync(id, token) : Task.FromResult<string>(null);
            await Task.WhenAll(tModel, tContent).ConfigureAwait(false);
            var preview = _factoryProcessor.PreviewFactory(tModel.Result.Blob);
            var result = await preview.ConvertFileToWebsitePreviewAsync(0, token).ConfigureAwait(false);
            var model = tModel.Result;
            if (model == null)
            {
                return NotFound();
            }
            return Ok(
                new
                {
                    details = model,
                    content = tContent.Result,
                    preview = result
                });
        }
    }
}