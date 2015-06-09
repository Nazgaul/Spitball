using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class BoxController : ApiController
    {
        public ApiServices Services { get; set; }
        public IZboxCacheReadService ZboxReadService { get; set; }
        public IZboxReadSecurityReadService ZboxReadSecurityService { get; set; }
        public IZboxWriteService ZboxWriteService { get; set; }

        // GET api/Box
        public async Task<HttpResponseMessage> Get(long id)
        {
            try
            {
                var query = new GetBoxQuery(id);
                var tResult = ZboxReadService.GetBox2(query);
                var tUserType = ZboxReadSecurityService.GetUserStatusToBoxAsync(id, User.GetCloudentsUserId());
                await Task.WhenAll(tResult, tUserType);
                var result = tResult.Result;
                result.UserType = tUserType.Result;

                return Request.CreateResponse(new
                {
                    result.Name,
                    result.BoxType,
                    result.UserType,
                    Professor = result.ProfessorName,
                    result.CourseId

                });
            }
            catch (BoxAccessDeniedException)
            {
                return Request.CreateUnauthorizedResponse();
            }
            catch (BoxDoesntExistException)
            {
                return Request.CreateNotFoundResponse();
            }

        }
        public HttpResponseMessage Post(CreateBoxRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            try
            {
                var userId = User.GetCloudentsUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = ZboxWriteService.CreateBox(command);
                return Request.CreateResponse(result);

            }
            catch (BoxNameAlreadyExistsException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "box already exists");
            }
        }

        [Route("api/box/academic")]
        [HttpPost]
        public HttpResponseMessage CreateAcademicBox(CreateAcademicBoxRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var universityId = User.GetUniversityId();

            if (!universityId.HasValue)
            {
                return Request.CreateBadRequestResponse();
            }
            var guid = GuidEncoder.TryParseNullableGuid(model.DepartmentId);
            if (!guid.HasValue)
            {
                return Request.CreateBadRequestResponse();
            }
            try
            {

                var userId = User.GetCloudentsUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, guid.Value);
                var result = ZboxWriteService.CreateBox(command);
                return Request.CreateResponse(new {result.Url, result.Id});
            }
            catch (BoxNameAlreadyExistsException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "box already exists");

            }
        }



        [HttpGet]
        [Route("api/box/{id:long}/items")]
        public async Task<HttpResponseMessage> Items(long id, Guid? tabId, int page,int sizePerPage = 20)
        {
            //TODO: Claim to check box permission
            var query = new GetBoxItemsPagedQuery(id, tabId, page, sizePerPage);
            var result = await ZboxReadService.GetBoxItemsPagedAsync(query) ?? new List<Zbox.ViewModel.Dto.ItemDtos.ItemDto>();
            return Request.CreateResponse(result.Select(s => new
            {
                s.Name,
                s.Type,
                //s.Thumbnail,
                s.Id,
                s.Source,
                //views = s.NumOfViews,
                //downloads = s.NumOfDownloads,
                //likes = s.NumOfDownloads,
                creationTime = s.Date,
                s.Owner,
                s.OwnerId,
                s.Url
            }));
        }


        [Route("api/box/rename")]
        [HttpPost]
        public HttpResponseMessage Rename(BoxRenameRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var userId = User.GetCloudentsUserId();
            try
            {
                var commandBoxName = new ChangeBoxInfoCommand(model.Id, userId, model.Name,
                    model.Professor, model.Course, null, null);
                ZboxWriteService.ChangeBoxInfo(commandBoxName);
                return Request.CreateResponse();
            }
            catch (UnauthorizedAccessException)
            {
                return Request.CreateUnauthorizedResponse("You don't have permission");
            }
           
           
        }

        [Route("api/box/follow")]
        [HttpPost]
        public async Task<HttpResponseMessage> Follow(FollowRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new SubscribeToSharedBoxCommand(User.GetCloudentsUserId(), model.BoxId);
            await ZboxWriteService.SubscribeToSharedBoxAsync(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var userId = User.GetCloudentsUserId();
            var command = new UnFollowBoxCommand(id, userId, false);
            await ZboxWriteService.UnFollowBoxAsync(command);
            return Request.CreateResponse();
        }

        [Route("api/box/invite")]
        [HttpPost]
        public async Task<HttpResponseMessage> Invite(InviteToBoxRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var userId = User.GetCloudentsUserId();
            var shareCommand = new ShareBoxCommand(model.BoxId, userId, model.Recipients);
            
            await ZboxWriteService.ShareBoxAsync(shareCommand);
            return Request.CreateResponse();
        }
    }
}
