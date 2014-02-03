using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Zbang.Zbox.Infrastructure.Url;
using Zbang.Zbox.ReadServices;
using ZboxQuery = Zbang.Zbox.ViewModel.Queries;
using ZboxDto = Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ApiViewModel.Queries;
using Zbang.Zbox.ApiViewModel.DTOs;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.WebApi.Models;
using Zbang.Zbox.WebApi.Helpers;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData;
using System.Web.Security;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Domain.Commands;
using System.IO;
using System.Globalization;
using System.Threading.Tasks;

namespace Zbang.Zbox.WebApi.Controllers
{
    [RoutingPrefix(UriPrefix = "V1")]
    public partial class ZboxController : ApiController
    {
        protected readonly IZboxApiReadService m_ZboxReadService;
        protected readonly IZboxWriteService m_ZboxWriteService;
        protected readonly IMembershipService m_MembershipService;
        protected readonly IFacebookAuthenticationService m_FacebookService;
        protected readonly IBlobProvider m_BlobProvider;
        protected readonly IShortCodesCache m_ShortToLongCode;

        public ZboxController(IZboxApiReadService zboxReadService, IZboxWriteService zboxWriteService, IMembershipService membershipService,
            IFacebookAuthenticationService facebookService, IBlobProvider blobProvider, IShortCodesCache shortToLongCode)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_MembershipService = membershipService;
            m_FacebookService = facebookService;
            m_BlobProvider = blobProvider;
            m_ShortToLongCode = shortToLongCode;
        }

        [NonAction]
        protected string CreateToken(UserToken userDetails)
        {
            var serialize = new SerializeData<UserToken>();
            var bytes = serialize.SerializeBinary(userDetails);
            return MachineKey.Encode(bytes, MachineKeyProtection.All);
        }

        [NonAction]
        private long GetUserId()
        {
            return User.Identity.IsAuthenticated ? Convert.ToInt64(User.Identity.Name) : -1L;
        }

        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}")]
        //[HttpGet]
        //[ActionName("Box")]
        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //public HttpResponseMessage GetBox(string boxid)
        //{
        //    var userid = User.Identity.IsAuthenticated ? Convert.ToInt64(User.Identity.Name) : -1L;
        //    try
        //    {
        //        var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
        //        var query = new GetBoxQuery(boxId, userid);

        //        var retVal = m_ZboxReadService.GetBox(query);
        //        return Request.CreateZboxOkResult(retVal);
        //    }

        //    catch (BoxDoesntExistException)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.NotFound, "box doesn't exist");
        //    }
        //}

        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Users")]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        public HttpResponseMessage GetBoxUsers(string boxid)
        {
            var userid = User.Identity.IsAuthenticated ? Convert.ToInt64(User.Identity.Name) : -1L;

            var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
            var query = new ZboxQuery.GetBoxUsersQuery(boxId, userid);
            var retVal = m_ZboxReadService.GetBoxUsers(query);
            return Request.CreateZboxOkResult(retVal.ToList());
        }

        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [RoutingAttribute(UriTemplate = "Boxes")]
        public HttpResponseMessage GetUserBoxes()
        {
            var userid = Convert.ToInt64(User.Identity.Name);

            var query = new GetBoxesQuery(userid);
            var result = m_ZboxReadService.GetBoxes(query);
            return Request.CreateZboxOkResult(result.ToList());
        }

        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items/{itemid}")]
        [HttpGet]
        [ActionName("Item")]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public HttpResponseMessage GetBoxItem(string boxid, string itemid)
        {
            var userid = User.Identity.IsAuthenticated ? Convert.ToInt64(User.Identity.Name) : -1L;
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);

                var query = new ZboxQuery.GetItemQuery(userid, itemId, boxId);
                var result = m_ZboxReadService.GetBoxItem(query);

                return Request.CreateZboxOkResult(result);
            }
            catch (ItemNotFoundException)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.NotFound, "Item not found on box");
            }
        }

        //[RoutingAttribute(UriTemplate = "User")]
        //[ZboxAuthorize]
        //public HttpResponseMessage GetUser()
        //{
        //    var userid = GetUserId();

        //    try
        //    {
        //        var query = new ZboxQuery.GetUserDetailsQuery(userid);
        //        var result = m_ZboxReadService.GetUserDetails(query);
        //        return Request.CreateZboxOkResult(result);
        //    }
        //    catch (ArgumentNullException)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.NotFound, "User not found");
        //    }
        //}


        [RoutingAttribute(UriTemplate = "Friend/{friendid}/Boxes")]
        [ZboxAuthorize]
        [HttpGet]
        public HttpResponseMessage GetFriendBoxes(long friendid)
        {
            var userid = GetUserId();

           // var friendId = m_ShortToLongCode.ShortCodeToLong(friendid, ShortCodesType.User);
            var userType = m_ZboxReadService.GetUserType(new ZboxQuery.GetUserTypeQuery(friendid));
            var query = new GetFriendBoxesQuery(friendid, userid, userType);
            var result = m_ZboxReadService.GetBoxes(query);
            return Request.CreateZboxOkResult(result.ToList());

        }

        //[RoutingAttribute(UriTemplate = "Friends")]
        //[ZboxAuthorize]
        //[HttpGet]
        //public HttpResponseMessage GetFriends()
        //{
        //    var userid = GetUserId();

        //    var query = new ZboxQuery.GetUserFriendsQuery(userid);
        //    var result = m_ZboxReadService.GetUserFriends(query);
        //    return Request.CreateZboxOkResult(result.ToList());

        //}

        //[Description("Get box items ( files, links etc.)")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items")]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        [HttpGet]
        public HttpResponseMessage GetBoxItems(string boxid)
        {
            var userid = GetUserId();

            var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
            var query = new ZboxQuery.GetBoxItemsPagedQuery(boxId, userid);
            var result = m_ZboxReadService.GetBoxItems(query);
            return Request.CreateZboxOkResult(result.ToList());
        }

        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items/{itemid}/Comments")]
        //[HttpGet]
        //[ActionName("ItemComments")]
        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //public HttpResponseMessage GetItemComments(string boxid, string itemid)
        //{
        //    var userid = GetUserId();

        //    var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
        //    var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
        //    var query = new ZboxQuery.GetItemCommentsQuery(itemId, userid);
        //    var result = m_ZboxReadService.GetItemComments(query);
        //    return Request.CreateZboxOkResult(result.ToList());
        //}

        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Comments")]
        [HttpGet]
        [ZboxAuthorize(IsAuthenticationRequired = false)]
        public HttpResponseMessage GetBoxComments(string boxid)
        {
            var userid = GetUserId();
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var query = new ZboxQuery.GetBoxCommentsQuery(boxId, userid);
                var result = m_ZboxReadService.GetBoxComments(query).ToList();
                return Request.CreateZboxOkResult(result);
            }
            catch (BoxDoesntExistException)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.NotFound, "box doesn't exist");
            }
        }



        [HttpPost]
        [RoutingAttribute(UriTemplate = "LogIn")]
        public HttpResponseMessage LogInUserMembership([FromBody] LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }
            Guid membershipUserId;
            if (m_MembershipService.ValidateUser(model.Email, model.Password, out membershipUserId) != Infrastructure.Enums.LogInStatus.Success)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest,
                                                          "username or password is incorrect");
            }
            var query = new ZboxQuery.GetUserByMembershipQuery(membershipUserId);
            var result = m_ZboxReadService.GetUserDetailsByMembershipId(query);
            var userDetails = new UserToken { UserId = result.Uid, ExpireTokenTime = DateTime.MaxValue };
            var token = CreateToken(userDetails);
            return Request.CreateZboxOkResult(token);
        }

        [RoutingAttribute(UriTemplate = "LogIn/facebook")]
        [HttpPost]
        public async Task<HttpResponseMessage> LogInUserFacebook([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "provide token");
            }
            var user = await m_FacebookService.FacebookLogIn(token);
            if (user == null)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "token incorrect");
            }
            try
            {
                var command = new CreateFacebookUserCommand(user.id, user.email, user.name,
                    user.Image, user.LargeImage, null);
                var commandResult = m_ZboxWriteService.CreateUser(command) as CreateFacebookUserCommandResult;
                var userDetails = new UserToken { UserId = commandResult.User.Id, ExpireTokenTime = DateTime.UtcNow.AddMinutes(30) };
                var zboxtoken = CreateToken(userDetails);
                return Request.CreateZboxOkResult(zboxtoken);
            }
            catch (WebException)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.InternalServerError, "Problem with getting facebook data");
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "email")
                {
                    return Request.CreateZboxErrorResponse(HttpStatusCode.BadGateway, "You need to ask facebook for email permission");
                }
                return Request.CreateZboxErrorResponse(HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        //[Routing(UriTemplate = "LogIn/Verify")]
        //[HttpPost]
        //public HttpResponseMessage LogInVerify([FromBody] LogInVerify model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateZboxErrorResponse(ModelState);
        //    }
        //    try
        //    {
        //        MachineKey.Encode(new byte[0], MachineKeyProtection.Validation);// .net 4 need to encode before decode - dont know why. need to check on .net 4.5
        //        var bytes = MachineKey.Decode(model.Token, MachineKeyProtection.Validation);
        //        var useruId = Encoding.UTF8.GetString(bytes);
        //        var userId = m_ShortToLongCode.ShortCodeToLong(useruId, ShortCodesType.User);
        //        var command = new VerifyEmailCommand(userId);
        //        m_ZboxWriteService.VerifyEmail(command);
        //        var userDetails = new UserToken { UserId = userId, ExpireTokenTime = DateTime.MaxValue };
        //        var authToken = CreateToken(userDetails);
        //        return Request.CreateZboxOkResult<string>(authToken);
        //    }
        //    catch (ArgumentException)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.InternalServerError, "Problem with token");

        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("LogInVerify token = " + model, ex);
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.InternalServerError, string.Empty);
        //    }
        //}

        //[Description("Request a url to download specific blob")]
        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items/{itemid}/readblob")]
        //[ZboxAuthorize(IsAuthenticationRequired = false)]
        //[HttpGet]
        //public HttpResponseMessage GetBlobReadPermission(string boxid, string itemid)
        //{
        //    var userid = GetUserId();
        //    var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
        //    var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
        //    var query = new ZboxQuery.GetItemQuery(userid, itemId, boxId);

        //    var result = m_ZboxReadService.GetBoxItem(query) as FileDto;
        //    if (result == null)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "This item is a link");

        //    }
        //    var blob = m_BlobProvider.GetFile(result.BlobName);
        //    var sharedAccessSignature = m_BlobProvider.GenerateSharedAccessReadPermissionBlobFiles(blob, 20);
        //    return Request.CreateZboxOkResult(sharedAccessSignature);
        //}

        //[Description("Request a url to upload to a specific blob - obselete. use Boxes/{boxid}/Item Post")]
        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}/Blob/{blobName}")]
        //[ZboxAuthorize]
        //[HttpGet]
        //public HttpResponseMessage GetBlobUploadPermission(string boxid, string blobName)
        //{
        //    var userid = GetUserId();

        //    var query = new GetBoxesQuery(userid);
        //    var result = m_ZboxReadService.GetBoxes(query);
        //    if (!result.Any(w => w.Uid == boxid))
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, "Don't have permission to upload to that box");
        //    }

        //    var blob = m_BlobProvider.GetFile(blobName);
        //    if (blob.Exists())
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.Conflict, "Blob with that name already exists");
        //    }
        //    using (var ms = new MemoryStream())
        //    {
        //        blob.UploadFromStream(ms);
        //    }
        //    //blob.UploadText(string.Empty);
        //    var contentType = ExtensionToMimeConvention.GetMimeType(Path.GetExtension(blobName));
        //    blob.Metadata[BlobProvider.BlobMetadataUseridKey] = userid.ToString(CultureInfo.InvariantCulture);
        //    blob.SetMetadata();
        //    blob.Properties.ContentType = contentType;
        //    blob.SetProperties();
        //    var sharedAccessSignature = m_BlobProvider.GenerateSharedAccessWritePermissionBlobFiles(blob, 20);
        //    return Request.CreateZboxOkResult(sharedAccessSignature);

        //}

        //[Description("Get user wall")]
        [RoutingAttribute(UriTemplate = "Wall")]
        [ZboxAuthorize]
        [HttpGet]
        public HttpResponseMessage GetUserWall()
        {
            var userid = GetUserId();

            var query = new GetWallQuery(userid);
            var result = m_ZboxReadService.GetUserWall(query);
            return Request.CreateZboxOkResult(result.ToList());

        }

    }
}
