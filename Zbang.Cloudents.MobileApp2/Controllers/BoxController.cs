using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Cloudents.MobileApp2.Models;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
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

        // GET api/Box
        //public async Task<HttpResponseMessage> Get(long id)
        //{
        //    try
        //    {
        //        var query = new GetBoxQuery(id);
        //        var tResult = ZboxReadService.GetBox2(query);
        //        //TODO: put claim
        //        var tUserType = ZboxReadSecurityService.GetUserStatusToBoxAsync(id, User.GetCloudentsUserId());
        //        await Task.WhenAll(tResult, tUserType);
        //        var result = tResult.Result;
        //        result.UserType = tUserType.Result;
                

        //        return Request.CreateResponse(new
        //        {
        //            result.Name,
        //            result.BoxType,
        //            result.UserType,
        //            result.ProfessorName,
        //            result.CourseId
        //        });
        //    }
        //    catch (BoxAccessDeniedException)
        //    {
        //        return Request.CreateUnauthorizedResponse();
        //    }
        //    catch (BoxDoesntExistException)
        //    {
        //        return Request.CreateNotFoundResponse();
        //    }
        //    catch (Exception ex)
        //    {
        //        Services.Log.Error(string.Format("Box Index id {0}", id), ex);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("error"));
        //    }

        //}



        [HttpGet]
        [Route("api/box/{boxId:long}/items")]
        public async Task<HttpResponseMessage> Items(long boxId, Guid? tabId, int page)
        {
            //TODO: Claim to check box permission
            var query = new GetBoxItemsPagedQuery(boxId, tabId, page, 20);
            var result = await ZboxReadService.GetBoxItemsPagedAsync(query) ?? new List<Zbox.ViewModel.Dto.ItemDtos.ItemDto>();
            return Request.CreateResponse(result.Select(s => new { s.Name, s.Thumbnail, s.Id, views = s.NumOfViews, likes = s.NumOfDownloads }));
            //Services.Log.Info("Hello from custom controller!");
            //return "Hello";
        }

    }
}
