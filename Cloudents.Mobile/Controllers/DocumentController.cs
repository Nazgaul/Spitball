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
    }
}
