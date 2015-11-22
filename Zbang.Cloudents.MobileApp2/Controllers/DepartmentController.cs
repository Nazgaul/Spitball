using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Library;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    public class DepartmentController : ApiController
    {
        public ApiServices Services { get; set; }

        public IZboxCacheReadService ZboxReadService { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }
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
            var result = await ZboxReadService.GetLibraryNodeAsync(query);
            return Request.CreateResponse(result);
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
                ZboxWriteService.CreateDepartment(command);
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
