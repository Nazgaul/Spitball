﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DocumentController : Controller
    {
        private readonly IReadRepositoryAsync<DocumentDto, long> _repository;
        private readonly Lazy<IDocumentSearch> _documentSearch;
        private readonly IFactoryProcessor _factoryProcessor;
        public DocumentController(IReadRepositoryAsync<DocumentDto, long> repository, Lazy<IDocumentSearch> documentSearch, IFactoryProcessor factoryProcessor)
        {
            _repository = repository;
            _documentSearch = documentSearch;
            _factoryProcessor = factoryProcessor;
        }

        public async Task<IActionResult> Get(long id, bool? firstTime, CancellationToken token)
        {
            var tModel = _repository.GetAsync(id, token);
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
            return Json(
                new
                {
                    details = model,
                    content = tContent.Result,
                    preview = result
                });
        }
    }
}