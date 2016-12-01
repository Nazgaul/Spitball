using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries;

namespace Zbang.Cloudents.MobileApp.Controllers
{
    [Authorize]
    [MobileAppController]
    public class BoxController : ApiController
    {
        private readonly IZboxCacheReadService m_ZboxReadService;
        private readonly IZboxReadSecurityReadService m_ZboxReadSecurityService;
        private readonly IZboxWriteService m_ZboxWriteService;

        public BoxController(IZboxCacheReadService zboxReadService, IZboxReadSecurityReadService zboxReadSecurityService, IZboxWriteService zboxWriteService)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxReadSecurityService = zboxReadSecurityService;
            m_ZboxWriteService = zboxWriteService;



        }


        //[VersionedRoute("api/box", 2)]
        [HttpGet]
        [Route("api/box"), ActionName("GetBox")]
        public async Task<HttpResponseMessage> GetBoxAsync(long id, int numberOfPeople = 6)
        {
            try
            {

                var query = new GetBoxQuery(id);
                var tResult = m_ZboxReadService.GetBoxMetaWithMembersAsync(query, numberOfPeople);
                var tUserType = m_ZboxReadSecurityService.GetUserStatusToBoxAsync(id, User.GetUserId());
                await Task.WhenAll(tResult, tUserType);
                var result = tResult.Result;
                result.UserType = tUserType.Result;

                return Request.CreateResponse(new
                {
                    result.Name,
                    result.BoxType,
                    result.UserType,
                    Professor = result.ProfessorName,
                    CourseCode = result.CourseId,
                    ShortUrl = UrlConst.BuildShortBoxUrl(new Base62(id).ToString()),
                    result.Members,
                    result.Items,
                    result.People

                });
            }
            catch (BoxAccessDeniedException)
            {

                TraceLog.WriteInfo($"userid: {User.GetUserId()} request box {id}");
                return Request.CreateUnauthorizedResponse();
            }
            catch (BoxDoesntExistException)
            {
                return Request.CreateNotFoundResponse();
            }

        }

        [HttpPost]
        [Route("api/box")]
        public async Task<HttpResponseMessage> PostAsync(CreateBoxRequest model)
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
                var userId = User.GetUserId();
                var command = new CreateBoxCommand(userId, model.BoxName);
                var result = await m_ZboxWriteService.CreateBoxAsync(command);
                return Request.CreateResponse(new
                {
                    result.Url,
                    result.Id,
                    shortUrl = UrlConst.BuildShortBoxUrl(new Base62(result.Id).ToString())
                });

            }
            catch (BoxNameAlreadyExistsException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "box already exists");
            }
        }

        [Route("api/box/academic")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateAcademicBoxAsync(CreateAcademicBoxRequest model)
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

                var userId = User.GetUserId();

                var command = new CreateAcademicBoxCommand(userId, model.CourseName,
                                                           model.CourseId, model.Professor, guid.Value, universityId.Value);
                var result = await m_ZboxWriteService.CreateBoxAsync(command);
                return Request.CreateResponse(new
                {
                    result.Url,
                    result.Id,
                    shortUrl = UrlConst.BuildShortBoxUrl(new Base62(result.Id).ToString())
                });
            }
            catch (BoxNameAlreadyExistsException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "box already exists");

            }
        }



        [HttpGet]
        [Route("api/box/{id:long}/items"), ActionName("Items")]
        public async Task<HttpResponseMessage> ItemsAsync(long id, Guid? tabId, int page, int sizePerPage = 20)
        {
            var query = new GetBoxItemsPagedQuery(id, tabId, page, sizePerPage);
            var result = await m_ZboxReadService.GetWebServiceBoxItemsPagedAsync(query) ?? new List<Zbox.ViewModel.Dto.ItemDtos.ItemDto>();
            return Request.CreateResponse(result.Select(s => new
            {
                s.Name,
                s.Type,
                s.Id,
                s.Source,
                creationTime = s.Date,
                s.Owner,
                s.OwnerId,
                s.Url
            }));
        }

        [HttpGet]
        [Route("api/box/{id:long}/quizzes"), ActionName("Quizzes")]
        public async Task<HttpResponseMessage> QuizzesAsync(long id, int page, int sizePerPage = 20)
        {
            var query = new GetBoxQuizesPagedQuery(id, page, sizePerPage);
            var result = await m_ZboxReadService.GetBoxQuizesAsync(query);
            return Request.CreateResponse(result.Where(w => w.Publish).Select(s => new
            {
                s.Name,
                s.Id
            }));

        }

        [HttpGet, Route("api/box/{id:long}/members"), ActionName("Members")]
        public async Task<HttpResponseMessage> MembersAsync(long id, int page, int sizePerPage = 20)
        {
            var result = await m_ZboxReadService.GetBoxMembersAsync(new GetBoxQuery(id, page, sizePerPage));
            return Request.CreateResponse(result.Select(s => new
            {
                s.Id,
                s.Image,
                s.Name
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
            var userId = User.GetUserId();
            try
            {
                var commandBoxName = new ChangeBoxInfoCommand(model.Id, userId, model.Name,
                    model.Professor, model.Course, null, null);
                m_ZboxWriteService.ChangeBoxInfo(commandBoxName);
                return Request.CreateResponse();
            }
            catch (UnauthorizedAccessException)
            {
                return Request.CreateUnauthorizedResponse("You don't have permission");
            }


        }

        [Route("api/box/follow")]
        [HttpPost]
        public HttpResponseMessage FollowAsync(FollowRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new SubscribeToSharedBoxCommand(User.GetUserId(), model.BoxId);
            m_ZboxWriteService.SubscribeToSharedBox(command);
            return Request.CreateResponse();
        }

        [HttpDelete]
        [Route("api/box"), ActionName("Delete")]
        public async Task<HttpResponseMessage> DeleteAsync(long id)
        {
            var userId = User.GetUserId();
            var command = new UnFollowBoxCommand(id, userId, false);
            await m_ZboxWriteService.UnFollowBoxAsync(command);
            return Request.CreateResponse();
        }

        [Route("api/box/invite"), ActionName("Invite")]
        [HttpPost]
        public async Task<HttpResponseMessage> InviteAsync(InviteToBoxRequest model)
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
                var userId = User.GetUserId();
                var shareCommand = new ShareBoxCommand(model.BoxId, userId, model.Recipients);

                await m_ZboxWriteService.ShareBoxAsync(shareCommand);
                return Request.CreateResponse();
            }
            catch (UnauthorizedAccessException)
            {
                return Request.CreateUnauthorizedResponse();
            }
        }
    }
}
