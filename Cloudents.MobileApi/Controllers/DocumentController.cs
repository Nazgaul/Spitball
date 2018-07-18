using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Document controller
    /// </summary>
    [Route("api/[controller]")]
    public class DocumentController : Controller
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
        [HttpGet]
        public async Task<IActionResult> GetDataAsync(long id, CancellationToken token)
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
        [HttpGet]
        public IActionResult Get(string blob)
        {
            if (string.IsNullOrEmpty(blob))
            {
                return BadRequest();
            }
            var blobUrl = _blobProviderFiles.GenerateSharedAccessReadPermission(blob, 20);
            return Ok(new
            {
                blobUrl
            });
        }
    }
}
