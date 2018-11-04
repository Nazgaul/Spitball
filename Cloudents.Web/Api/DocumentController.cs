using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IBlobProvider<DocumentContainer> _blobProvider;
        private readonly IStringLocalizer<DocumentController> _localizer;

        public DocumentController(IQueryBus queryBus,
             ICommandBus commandBus, UserManager<User> userManager, IBlobProvider<DocumentContainer> blobProvider, SignInManager<User> signInManager, IStringLocalizer<DocumentController> localizer)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _blobProvider = blobProvider;
            _signInManager = signInManager;
            _localizer = localizer;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id, CancellationToken token)
        {
            var query = new DocumentById(id);
            var tModel = _queryBus.QueryAsync<DocumentDetailDto>(query, token);
            //var tContent = firstTime.GetValueOrDefault() ?
            //    _documentSearch.Value.ItemContentAsync(id, token) : Task.FromResult<string>(null);

            //https://stackoverflow.com/questions/6396378/c-sharp-linq-orderby-numbers-that-are-string-and-you-cannot-convert-them-to-int
            var filesTask = _blobProvider.FilesInDirectoryAsync("preview-", query.Id.ToString(), token);

            await Task.WhenAll(tModel, filesTask);

            var model = tModel.Result;
            var files = filesTask.Result;
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

        [HttpPost, Authorize]
        public async Task<ActionResult> CreateDocumentAsync([FromBody]CreateDocumentRequest model, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var command = new CreateDocumentCommand(model.BlobName, model.Name, model.Type,
                model.Course, model.Tags, userId, model.Professor);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }


        /// <summary>
        /// Search document vertical result
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ilSearchProvider"></param>
        /// <param name="token"></param>
        /// <param name="universityId">This params comes from claim and not from api - value is ignored</param>
        /// <param name="country">This params comes from server internal process - value is ignored</param>
        /// <returns></returns>
        [HttpGet(Name = "DocumentSearch")]
        public async Task<WebResponseWithFacet<DocumentFeedDto>> SearchDocumentAsync([FromQuery] DocumentRequest model,
            [ClaimModelBinder(AppClaimsPrincipalFactory.University)] Guid? universityId,
            [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromServices] IDocumentSearch ilSearchProvider,
            CancellationToken token)
        {

            model = model ?? new DocumentRequest();

            var query = new DocumentQuery(model.Course, universityId, model.Query, country, model.Page.GetValueOrDefault());
            

            var coursesTask = Task.FromResult<IEnumerable<CourseDto>>(null);

            if (_signInManager.IsSignedIn(User))
            {
                var userId = _signInManager.UserManager.GetLongUserId(User);
                var dbQuery = new CoursesQuery(userId);
                coursesTask = _queryBus.QueryAsync(dbQuery, token);
            }

            var resultTask = ilSearchProvider.SearchDocumentsAsync(query, token);
            await Task.WhenAll(coursesTask, resultTask);
            var result = resultTask.Result;
            var p = result.Result?.ToList();
            string nextPageLink = null;
            if (p?.Any() == true)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }
            var filters = new List<IFilters>();

            if (result.Facet != null)
            {
                filters.Add(

                    new Filters<string>(nameof(SearchRequest.Source), _localizer["Sources"],
                        result.Facet.Select(s => new KeyValuePair<string, string>(s, s)))

                );
            }

            if (coursesTask.Result != null)
            {
                filters.Add(new Filters<string>(nameof(SearchRequest.Course),
                    _localizer["CoursesFilterTitle"],
                    coursesTask.Result.Select(s => new KeyValuePair<string, string>(s.Name, s.Name))));
            }
            return new WebResponseWithFacet<DocumentFeedDto>
            {
                Result = p,
                Sort = EnumExtension.GetValues<SearchRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
                Filters = filters,
                NextPageLink = nextPageLink
            };




        }
    }
}