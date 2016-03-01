using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [MobileAppController]
    [Authorize]
    public class DepartmentController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IZboxWriteService m_ZboxWriteService;

        public DepartmentController(IZboxCacheReadService zboxReadService, IZboxWriteService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
        }

        // GET api/Department
        public async Task<HttpResponseMessage> Get(string departmentId)
        {
            var guid = GuidEncoder.TryParseNullableGuid(departmentId);
            var universityId = User.GetUniversityDataId();

            if (!universityId.HasValue)
            {
                return Request.CreateBadRequestResponse();
            }
            var query = new GetLibraryNodeQuery(universityId.Value, guid, User.GetCloudentsUserId());
            var result = await m_ZboxReadService.GetLibraryNodeAsync(query);
            return Request.CreateResponse(new
            {
                result.Boxes,
                result.Details,
                Nodes = result.Nodes.Where(w => w.State != Zbox.Infrastructure.Enums.LibraryNodeSettings.Closed)
            });
        }

        public HttpResponseMessage Post(CreateLibraryRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var universityId = User.GetUniversityDataId();

            if (!universityId.HasValue)
            {
                return Request.CreateBadRequestResponse();
            }

            try
            {
                var parentId = GuidEncoder.TryParseNullableGuid(model.ParentId);
                var command = new AddNodeToLibraryCommand(model.Name, universityId.Value, parentId, User.GetCloudentsUserId());
                m_ZboxWriteService.CreateDepartment(command);
                var result = new NodeDto { Id = command.Id, Name = model.Name, Url = command.Url };
                return Request.CreateResponse(result);
            }
            catch (DuplicateDepartmentNameException ex)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.Conflict, ex);
            }
            catch (BoxesInDepartmentNodeException ex)
            {
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.MethodNotAllowed, ex);
            }
        }

    }
}
