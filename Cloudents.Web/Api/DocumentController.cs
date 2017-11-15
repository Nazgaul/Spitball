using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/Document")]
    public class DocumentController : Controller
    {
        private readonly IReadRepositoryAsync<DocumentDto, long> _repository;
        private readonly Lazy<IDocumentSearch> _documentSearch;
        public DocumentController(IReadRepositoryAsync<DocumentDto, long> repository, Lazy<IDocumentSearch> documentSearch)
        {
            _repository = repository;
            _documentSearch = documentSearch;
        }

        public async Task<IActionResult> Get(long id, bool? firstTime, CancellationToken token)
        {
            var tModel = _repository.GetAsync(id, token);
            Task<string> tContent;
            if (firstTime.GetValueOrDefault())
            {
                tContent = _documentSearch.Value.ItemContentAsync(id, token);
                //TODO: need to bring content of blob
            }
            else
            {
                tContent = Task.FromResult<string>(null);
            }
            await Task.WhenAll(tModel, tContent).ConfigureAwait(false);

            var model = tModel.Result;
            if (model == null)
            {
                return NotFound();
            }
            return Json(
                new
                {
                    model,
                    content = tContent.Result
                });
        }
    }
}