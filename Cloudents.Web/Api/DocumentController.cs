using Cloudents.Core.Attributes;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Models;

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
        private readonly IProfileUpdater _profileUpdater;



        public DocumentController(IQueryBus queryBus,
             ICommandBus commandBus, UserManager<User> userManager,
            IBlobProvider<DocumentContainer> blobProvider,
            SignInManager<User> signInManager, IStringLocalizer<DocumentController> localizer,
            IProfileUpdater profileUpdater)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _userManager = userManager;
            _blobProvider = blobProvider;
            _signInManager = signInManager;
            _localizer = localizer;
            _profileUpdater = profileUpdater;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentPreviewResponse>> GetAsync(long id,
            [FromServices] IQueueProvider queueProvider,
            CancellationToken token)
        {
            var query = new DocumentById(id);
            var tModel = _queryBus.QueryAsync<DocumentDetailDto>(query, token);
            var filesTask = _blobProvider.FilesInDirectoryAsync("preview-", query.Id.ToString(), token);

            var tQueue = queueProvider.InsertMessageAsync(new UpdateDocumentNumberOfViews(id), token);
            await Task.WhenAll(tModel, filesTask, tQueue);

            var model = tModel.Result;
            var files = filesTask.Result;
            if (model == null)
            {
                return NotFound();
            }
            return new DocumentPreviewResponse(model, files);
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
            [ProfileModelBinder(ProfileServiceQuery.University | ProfileServiceQuery.Country | ProfileServiceQuery.Course)] UserProfile profile,
            [FromServices] IDocumentSearch ilSearchProvider,
            CancellationToken token)
        {
            model = model ?? new DocumentRequest();
            var query = new DocumentQuery(model.Course, profile, model.Term, 
                model.Page.GetValueOrDefault(), model.Filter?.Where(w => w.HasValue).Select(s => s.Value));


            var queueTask = _profileUpdater.AddTagToUser(model.Term, User, token);
           

            var resultTask = ilSearchProvider.SearchDocumentsAsync(query, token);
            await Task.WhenAll( resultTask, queueTask);
            var result = resultTask.Result;
            var p = result;
            string nextPageLink = null;
            if (p.Count > 0)
            {
                nextPageLink = Url.NextPageLink("DocumentSearch", null, model);
            }
            var filters = new List<IFilters>();

            //if (result.Facet != null)
            //{

            filters.Add(
                new Filters<string>(nameof(DocumentRequest.Filter), _localizer["TypeFilterTitle"],
                    EnumExtension.GetValues<DocumentType>().Where(w => w.GetAttributeValue<PublicValueAttribute>() != null)
                        .Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())))
            );
            // }

            if (profile.Courses != null)
            {
                filters.Add(new Filters<string>(nameof(DocumentRequest.Course),
                    _localizer["CoursesFilterTitle"],
                    profile.Courses.Select(s => new KeyValuePair<string, string>(s, s))));
            }
            return new WebResponseWithFacet<DocumentFeedDto>
            {
                Result = p.Select(s =>
                {
                    if (s.Url == null)
                    {
                        s.Url = Url.RouteUrl(SeoTypeString.Document, new
                        {
                            universityName = s.University,
                            courseName = s.Course,
                            id = s.Id,
                            name = s.Title
                        });
                    }

                    return s;
                }),
                //Sort = EnumExtension.GetValues<SearchRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
                Filters = filters,
                NextPageLink = nextPageLink
            };




        }
    }
}