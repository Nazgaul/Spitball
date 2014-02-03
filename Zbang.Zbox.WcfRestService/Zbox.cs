using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Security;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData;
using ZboxDto = Zbang.Zbox.ViewModel.DTOs;
using ZboxQuery = Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.WcfRestService.Models;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.ReadServices;

using Zbang.Zbox.ApiViewModel.DTOs;
using Zbang.Zbox.ApiViewModel.Queries;

namespace Zbang.Zbox.WcfRestService
{
    // Start the service and browse to http://<machine_name>:<port>/Service1/help to view the service's generated help page
    // NOTE: By default, a new instance of the service is created for each call; change the InstanceContextMode to Single if you want
    // a single instance of the service to process all calls.	
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class Zbox
    {
        readonly IZboxApiReadService m_ZboxReadService;
        readonly IZboxWriteService m_ZboxWriteService;
        readonly IMembershipService m_MembershipService;
        readonly IFacebookAuthenticationService m_FacebookService;
        readonly IBlobProvider m_BlobProvider;
        readonly IShortCodesCache m_ShortToLongCode;

        public Zbox(IZboxApiReadService zboxReadService, IZboxWriteService zboxWriteService, IMembershipService membershipService,
            IFacebookAuthenticationService facebookService, IBlobProvider blobProvider, IShortCodesCache shortToLongCode)
        {
            m_ZboxReadService = zboxReadService;
            m_ZboxWriteService = zboxWriteService;
            m_MembershipService = membershipService;
            m_FacebookService = facebookService;
            m_BlobProvider = blobProvider;
            m_ShortToLongCode = shortToLongCode;
        }

        #region WebGet
        [Description("Get box data")]
        [WebGet(UriTemplate = "Boxes/{boxid}")]
        public ResultDto<ZboxDto.BoxWithDetailDto> GetBox(string boxid)
        {
            if (string.IsNullOrEmpty(boxid))
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.BadRequest, "boxid is required");

            }
            var userid = GetUserId();
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var query = new ZboxQuery.GetBoxQuery(boxId, userid);

                var retVal = m_ZboxReadService.GetBox(query);
                return ResultDto<ZboxDto.BoxWithDetailDto>.GetSuccessResult(retVal);
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.Unauthorized, "User is not authorized to see box");

            }
            catch (BoxDoesntExistException)
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.NotFound, "box doesn't exist");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBox , userid {0} boxid {1} ", userid, boxid), ex);
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.InternalServerError, "");

            }
        }

        [Description("Get users connected to a specific box")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Users")]
        public ResultDto<List<ZboxDto.UserPublicSettingDto>> GetBoxUsers(string boxid)
        {
            var userid = GetUserId();
            if (string.IsNullOrEmpty(boxid))
            {
                return WriteOutgoingErrorResponse<List<ZboxDto.UserPublicSettingDto>>(HttpStatusCode.BadRequest, "boxid is required");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var query = new ZboxQuery.GetBoxUsersQuery(boxId, userid);
                var retVal = m_ZboxReadService.GetBoxUsers(query);
                return ResultDto<List<ZboxDto.UserPublicSettingDto>>.GetSuccessResult(retVal.ToList());
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<List<ZboxDto.UserPublicSettingDto>>(HttpStatusCode.Unauthorized, "User is not authorized to see box users");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBoxUsers , userid {0} boxid {1} ", userid, boxid), ex);
                return WriteOutgoingErrorResponse<List<ZboxDto.UserPublicSettingDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get file or url by id")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Items/{itemid}")]
        public ResultDto<ItemDto> GetBoxItem(string boxid, string itemid)
        {
            if (string.IsNullOrEmpty(boxid))
            {
                return WriteOutgoingErrorResponse<ItemDto>(HttpStatusCode.BadRequest, "boxid is required");
            }

            if (string.IsNullOrEmpty(itemid))
            {
                return WriteOutgoingErrorResponse<ItemDto>(HttpStatusCode.BadRequest, "itemid is required");
            }
            var userid = GetUserId();
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);

                var query = new ZboxQuery.GetItemQuery(userid, itemId, boxId);
                var result = m_ZboxReadService.GetBoxItem(query);
                //if (result.BoxUid == boxid)
                //{
                return ResultDto<ItemDto>.GetSuccessResult(result);
                //}
                //return WriteOutgoingErrorResponse<ItemDto>(HttpStatusCode.NotFound, "Item not found on box");
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<ItemDto>(HttpStatusCode.Unauthorized, "User is not authorized to see this item");
            }
            catch (ItemNotFoundException)
            {
                return WriteOutgoingErrorResponse<ItemDto>(HttpStatusCode.NotFound, "Item not found on box");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBoxItem , userid {0} boxid {1} itemid {2}", userid, boxid, itemid), ex);
                return WriteOutgoingErrorResponse<ItemDto>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get user data need to be authenticated")]
        [WebGet(UriTemplate = "User")]
        public ResultDto<ZboxDto.UserDto> GetUser()
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<ZboxDto.UserDto>(HttpStatusCode.Unauthorized, "Need to authenticate to see data");
            }
            try
            {
                var query = new ZboxQuery.GetUserDetailsQuery(userid);
                var result = m_ZboxReadService.GetUserDetails(query);
                return ResultDto<ZboxDto.UserDto>.GetSuccessResult(result);
            }
            catch (ArgumentNullException)
            {
                return WriteOutgoingErrorResponse<ZboxDto.UserDto>(HttpStatusCode.NotFound, "User not found");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetUser , userid {0}", userid), ex);
                return WriteOutgoingErrorResponse<ZboxDto.UserDto>(HttpStatusCode.InternalServerError, "");
            }

        }

        [Description("Get user owned and subscribes boxes, need to be authenticated")]
        [WebGet(UriTemplate = "Boxes")]
        public ResultDto<List<ApiBoxDto>> GetUserBoxes()
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<List<ApiBoxDto>>(HttpStatusCode.Unauthorized, "Need to authenticate to see data");
            }
            try
            {
                var query = new GetBoxesQuery(userid);
                var result = m_ZboxReadService.GetBoxes(query);
                return ResultDto<List<ApiBoxDto>>.GetSuccessResult(result.ToList());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetUserBoxes , userid {0}", userid), ex);
                return WriteOutgoingErrorResponse<List<ApiBoxDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get user friend/page boxes, need to be authenticated")]
        [WebGet(UriTemplate = "Friend/{friendid}/Boxes")]
        public ResultDto<List<ApiBoxDto>> GetFriendBoxes(string friendid)
        {
            var userid = GetUserId();

            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<List<ApiBoxDto>>(HttpStatusCode.Unauthorized, "Need to authenticate to see data");
            }
            if (string.IsNullOrWhiteSpace(friendid))
            {
                return WriteOutgoingErrorResponse<List<ApiBoxDto>>(HttpStatusCode.BadRequest, "friendId is required");
            }
            try
            {
                var friendId = m_ShortToLongCode.ShortCodeToLong(friendid, ShortCodesType.User);
                var userType = m_ZboxReadService.GetUserType(new ZboxQuery.GetUserTypeQuery(friendId));
                var query = new GetFriendBoxesQuery(friendId,userid,userType);
                var result = m_ZboxReadService.GetBoxes(query);
                return ResultDto<List<ApiBoxDto>>.GetSuccessResult(result.ToList());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetFriendBoxes , userid {0} friendid {1}", userid, friendid), ex);
                return WriteOutgoingErrorResponse<List<ApiBoxDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get user friend list")]
        [WebGet(UriTemplate = "Friends")]
        public ResultDto<List<UserDto>> GetFriends()
        {
            var userid = GetUserId();

            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<List<UserDto>>(HttpStatusCode.Unauthorized, "Need to authenticate to see data");
            }
            try
            {
                var query = new ZboxQuery.GetUserFriendsQuery(userid);
                var result = m_ZboxReadService.GetUserFriends(query);
                return ResultDto<List<UserDto>>.GetSuccessResult(result.ToList());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetFriends , userid {0}", userid), ex);
                return WriteOutgoingErrorResponse<List<UserDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get box items ( files, links etc.)")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Items")]
        public ResultDto<List<ItemDto>> GetBoxItems(string boxid)
        {
            if (string.IsNullOrEmpty(boxid))
            {
                return WriteOutgoingErrorResponse<List<ItemDto>>(HttpStatusCode.BadRequest, "boxid is required");
            }
            var userid = GetUserId();
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var query = new ZboxQuery.GetBoxItemsPagedQuery(boxId, userid);
                var result = m_ZboxReadService.GetBoxItems(query);
                return ResultDto<List<ItemDto>>.GetSuccessResult(result.ToList());
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<List<ItemDto>>(HttpStatusCode.Unauthorized, "User is not authorized to see box items");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBoxItems , userid {0} boxid {1} ", userid, boxid), ex);
                return WriteOutgoingErrorResponse<List<ItemDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get item comments")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Items/{itemid}/Comments")]
        public ResultDto<List<ApiCommentDto>> GetItemComments(string boxid, string itemid)
        {
            if (string.IsNullOrEmpty(boxid))
            {
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.BadRequest, "boxid is required");
            }
            if (string.IsNullOrEmpty(itemid))
            {
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.BadRequest, "itemid is required");
            }
            var userid = GetUserId();
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);

                var query = new ZboxQuery.GetItemCommentsQuery(itemId, userid, boxId);
                var result = m_ZboxReadService.GetItemComments(query);
                return ResultDto<List<ApiCommentDto>>.GetSuccessResult(result.ToList());

            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.Unauthorized, "User is not authorized to see comments");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetiCmnts , userid {0} boxid {1} itemid {2}", userid, boxid, itemid), ex);
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.InternalServerError, "");
            }

        }

        [Description("Get box comments")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Comments")]
        public ResultDto<List<ApiCommentDto>> GetBoxComments(string boxid)
        {
            if (string.IsNullOrEmpty(boxid))
            {
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.BadRequest, "boxid is required");
            }
            var userid = GetUserId();
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var query = new ZboxQuery.GetBoxCommentsQuery(boxId, userid);
                var result = m_ZboxReadService.GetBoxComments(query).ToList();
                return ResultDto<List<ApiCommentDto>>.GetSuccessResult(result);
            }
            catch (BoxDoesntExistException)
            {
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.NotFound, "box doesn't exist");
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.Unauthorized, "User is not authorized to see comments");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBoxComments , userid {0} boxid {1} ", userid, boxid), ex);
                return WriteOutgoingErrorResponse<List<ApiCommentDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Authorize user by username password")]
        [WebGet(UriTemplate = "LogIn?email={email}&password={password}")]
        public ResultDto<string> LogInUserMembership(string email, string password)
        {
            try
            {
                Guid membershipUserId;
                if (!m_MembershipService.ValidateUser(email, password, out membershipUserId))
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest,
                                                              "username or password is incorrect");
                }
                var query = new ZboxQuery.GetUserByMembershipQuery(membershipUserId);
                var result = m_ZboxReadService.GetUserDetailsByMembershipId(query);
                var userDetails = new UserToken { UserId = result, ExpireTokenTime = DateTime.MaxValue };
                var token = CreateToken(userDetails);
                return ResultDto<string>.GetSuccessResult(token);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api LogInUserMembership , email {0} ", email), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Authorize user by facebook token")]
        [WebGet(UriTemplate = "LogIn/facebook?token={facebooktoken}")]
        public ResultDto<string> LogInUserFacebook(string facebooktoken)
        {
            var user = m_FacebookService.FacebookLogIn(facebooktoken);
            if (user == null)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "token incorrect");
            }
            try
            {
                var command = new CreateFacebookUserCommand(user.id, user.email, user.name, "https://graph.facebook.com/" + user.id + "/picture");
                var commandResult = m_ZboxWriteService.CreateUser(command) as CreateFacebookUserCommandResult;
                var userDetails = new UserToken { UserId = commandResult.User.Id, ExpireTokenTime = DateTime.UtcNow.AddMinutes(30) };
                var zboxtoken = CreateToken(userDetails);
                return ResultDto<string>.GetSuccessResult(zboxtoken);
            }
            catch (WebException)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "Problem with getting facebook data");
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "email")
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadGateway, "You need to ask facebook for email permission");
                }
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api LogInUserFacebook , facebooktoken {0} ", facebooktoken), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");

            }
        }

        [Description("Request a url to download specific blob")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Items/{itemid}/readblob")]
        public ResultDto<string> GetBlobReadPermission(string boxid, string itemid)
        {
            var userid = GetUserId();
            var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
            var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
            try
            {
                var query = new ZboxQuery.GetItemQuery(userid, itemId, boxId);

                var result = m_ZboxReadService.GetBoxItem(query) as FileDto;
                if (result == null)
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "This item is a link");

                }
                var blob = m_BlobProvider.GetFile(result.BlobName);
                var sharedAccessSignature = m_BlobProvider.GenerateSharedAccessReadPermissionBlobFiles(blob, 20);
                return ResultDto<string>.GetSuccessResult(sharedAccessSignature);
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "User is not authorized to download this blob");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBlobReadPermission , userid {0} boxid {1} itemid {2} ", userid, boxid, itemid), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Request a url to upload to a specific blob - obselete. use Boxes/{boxid}/Item Post")]
        [WebGet(UriTemplate = "Boxes/{boxid}/Blob/{blobName}")]
        public ResultDto<string> GetBlobUploadPermission(string boxid, string blobName)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to upload to blob");
            }
            try
            {
                //if (string.IsNullOrEmpty(Path.GetExtension(blobName)))
                //{
                //    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "Blob name should specify an extension");
                //}
                var query = new GetBoxesQuery(userid);
                var result = m_ZboxReadService.GetBoxes(query);
                if (!result.Any(w => w.Uid == boxid))
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, "Don't have permission to upload to that box");
                }

                var blob = m_BlobProvider.GetFile(blobName);
                if (blob.Exists())
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.Conflict, "Blob with that name already exists");
                }
                blob.UploadText(string.Empty);
                var contentType = ExtensionToMimeConvention.GetMimeType(Path.GetExtension(blobName));
                blob.Metadata[BlobProvider.blobMetadataUseridKey] = userid.ToString(CultureInfo.InvariantCulture);
                blob.SetMetadata();
                blob.Properties.ContentType = contentType;
                blob.SetProperties();
                var sharedAccessSignature = m_BlobProvider.GenerateSharedAccessWritePermissionBlobFiles(blob, 20);
                return ResultDto<string>.GetSuccessResult(sharedAccessSignature);
            }
            catch (BoxAccessDeniedException)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "User is not authorized to download this blob");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetBlobUploadPermission , userid {0} boxid {1} blobName {2} ", userid, boxid, blobName), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Get user wall")]
        [WebGet(UriTemplate = "Wall")]
        public ResultDto<List<ApiTextDto>> GetUserWall()
        {
            var userid = GetUserId();

            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<List<ApiTextDto>>(HttpStatusCode.Unauthorized, "Need to authenticate to see data");
            }
            try
            {
                var query = new GetWallQuery(userid);
                var result = m_ZboxReadService.GetUserWall(query);
                return ResultDto<List<ApiTextDto>>.GetSuccessResult(result.ToList());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api GetUserWall , userid {0}", userid), ex);
                return WriteOutgoingErrorResponse<List<ApiTextDto>>(HttpStatusCode.InternalServerError, "");
            }
        }

        #endregion WebGet


      

        private ResultDto<T> WriteOutgoingErrorResponse<T>(HttpStatusCode code, string desc) where T : class
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = code;
            WebOperationContext.Current.OutgoingResponse.StatusDescription = desc;
            return ResultDto<T>.GetErrorResult(code, desc);
        }

        private long GetUserId()
        {
            var token = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Authorization];
            if (string.IsNullOrEmpty(token))
            {
                return -1; // user is unauthorized
            }
            try
            {
                var bytes = MachineKey.Decode(token, MachineKeyProtection.All);
                var serialize = new SerializeData<UserToken>();
                var userDetails = serialize.Deserialize(bytes);
                if (userDetails.ExpireTokenTime > DateTime.UtcNow)
                {
                    return userDetails.UserId;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        private string CreateToken(UserToken userDetails)
        {
            var serialize = new SerializeData<UserToken>();
            var bytes = serialize.Serialize(userDetails);
            return MachineKey.Encode(bytes, MachineKeyProtection.All);
        }

        private bool IsAuthorized(long userID)
        {
            return userID > 0;
        }
    }
}
