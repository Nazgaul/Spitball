using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <summary>
    /// Document controller
    /// </summary>
    [MobileAppController]
    public class DocumentController : ApiController
    {
        private readonly IBlobProvider<FilesContainerName> _blobProviderFiles;
        private readonly IReadRepositoryAsync<DocumentDto, long> _repository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blobProviderFiles"></param>
        /// <param name="repository"></param>
        public DocumentController(IBlobProvider<FilesContainerName> blobProviderFiles, IReadRepositoryAsync<DocumentDto, long> repository)
        {
            _blobProviderFiles = blobProviderFiles;
            _repository = repository;
        }

        /// <summary>
        /// Get spitball document data
        /// </summary>
        /// <param name="id">id of the document</param>
        /// <param name="token">cancellation token</param>
        /// <returns>name and source</returns>
        [Route("api/document/{id}")]
        public async Task<IHttpActionResult> GetDataAsync(long id, CancellationToken token)
        {
            var retVal = await _repository.GetAsync(id, token).ConfigureAwait(false);

            return Ok(new
            {
                retVal.Name,
                Source = retVal.Blob,
            });
        }

        /// <summary>
        /// Get download link of specific doc
        /// </summary>
        /// <param name="blob">blob name</param>
        /// <returns>link</returns>
        public IHttpActionResult Get(string blob)
        {
            if (string.IsNullOrEmpty(blob))
            {
                return BadRequest();
            }
            var blobUrl = _blobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
            return Ok(blobUrl);
        }

        //[Route("api/document/like")]
        //[HttpPost]
        //[Authorize]
        //public HttpResponseMessage LikeAsync(/*ItemLikeRequest model*/)
        //{
        //    //if (ModelState?.IsValid == false)
        //    //{
        //    //    return Request.CreateBadRequestResponse();
        //    //}
        //    //var id = _guidGenerator.GetId();
        //    //var command = new RateItemCommand(model.Id, User.GetUserId(), id);
        //    //await _zboxWriteService.RateItemAsync(command).ConfigureAwait(true);

        //    //if (model.Tags?.Any() == true)
        //    //{
        //    //    var z = new AssignTagsToDocumentCommand(model.Id, model.Tags, TagType.User);
        //    //    await _zboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
        //    //}

        //    return Request.CreateResponse();
        //}

        //[HttpGet]
        //[Route("api/document/upload")]
        //public string UploadLink(string blob, string mimeType)
        //{
        //    return _blobProviderFiles.GenerateSharedAccessWritePermission(blob, mimeType);
        //}

        //[Route("api/document/upload/commit")]
        //[HttpPost]
        //[Authorize]
        //public async Task<HttpResponseMessage> CommitFileAsync(FileUploadRequest model)
        //{
        //    if (model == null)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }
        //    if (!long.TryParse(model.BoxId, out var boxId))
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }

        //    var size = await _blobProviderFiles.SizeAsync(model.BlobName).ConfigureAwait(false);
        //    var command = new AddFileToBoxCommand(User.GetUserId(),
        //        boxId, model.BlobName,
        //           model.FileName,
        //            size, null, model.Question);
        //    var result = await _zboxWriteService.AddItemToBoxAsync(command).ConfigureAwait(false);

        //    if (!(result is AddFileToBoxCommandResult result2))
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "result is null");
        //    }

        //    var fileDto = new ItemDto
        //    {
        //        Id = result2.File.Id
        //        //Name = result2.File.Name

        //    };

        //    return Request.CreateResponse(fileDto);
        //}

        //[HttpPost]
        //[Authorize]
        //[Route("api/document/upload/dropbox")]
        //public async Task<HttpResponseMessage> DropBoxAsync(DropboxUploadRequest model)
        //{
        //    if (model == null)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateBadRequestResponse();
        //    }
        //    var userId = User.GetUserId();
        //    var extension = Path.GetExtension(model.Name);
        //    if (extension == null)
        //    {
        //        return Request.CreateBadRequestResponse("Can't upload file without extension");
        //    }
        //    var blobAddressUri = Guid.NewGuid().ToString().ToLower() + extension.ToLower();

        //    var size = 0L;

        //    try
        //    {
        //        await _blobProviderFiles.UploadFromLinkAsync(model.Url, blobAddressUri).ConfigureAwait(false);
        //        size = await _blobProviderFiles.SizeAsync(blobAddressUri).ConfigureAwait(false);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        Request.CreateUnauthorizedResponse("can't access dropbox api");
        //    }

        //    var command = new AddFileToBoxCommand(userId, model.BoxId, blobAddressUri,
        //       model.Name, size, null, model.Question);
        //    var result = await _zboxWriteService.AddItemToBoxAsync(command).ConfigureAwait(false);
        //    var result2 = result as AddFileToBoxCommandResult;
        //    if (result2 == null)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "result is null");
        //    }
        //    var fileDto = new ItemDto
        //    {
        //        Id = result2.File.Id,
        //        Source = blobAddressUri
        //        //Name = result2.File.Name,
        //    };
        //    return Request.CreateResponse(fileDto);
        //}
    }
}
