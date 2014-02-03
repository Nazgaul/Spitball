using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Web;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;

using Zbang.Zbox.WcfRestService.Models;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ApiViewModel.Queries;
using ZboxDto = Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.ShortUrl;

using Zbang.Zbox.ApiViewModel.DTOs;

namespace Zbang.Zbox.WcfRestService
{
    public partial class Zbox
    {
        [Description("Delete or unfollow specific box")]
        [WebInvoke(Method = "DELETE", UriTemplate = "Boxes/{boxid}")]
        public ResultDto<string> DeleteBox(string boxid)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var userboxes = m_ZboxReadService.GetBoxes(new GetBoxesQuery(userid));
                var box = userboxes.FirstOrDefault(w => w.Uid == boxid);
                if (box.MemberType == Infrastructure.Enums.UserRelationshipType.owner)
                {
                    var command = new DeleteBoxCommand(boxId, userid);
                    m_ZboxWriteService.DeleteBox(command);
                    return ResultDto<string>.GetSuccessResult(string.Empty);
                }
                if (box.MemberType == Infrastructure.Enums.UserRelationshipType.subscribe)
                {
                    var command = new DeleteUserFromBoxCommand(userid, userid, boxId);
                    m_ZboxWriteService.DeleteUserFromBox(command);
                    return ResultDto<string>.GetSuccessResult(string.Empty);
                }

                TraceLog.WriteError(string.Format("Rest api DeleteBox , userid {0} boxid {1} reach to not implemented", userid, boxid));
                return ResultDto<string>.GetErrorResult(HttpStatusCode.NotImplemented, string.Empty);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (BoxDoesntExistException)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Gone, "Box doesn't exists");
            }
            catch (InvalidOperationException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api DeleteBox , userid {0} boxid {1}", userid, boxid), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("follow specific box")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{boxid}/Follow")]
        public ResultDto<string> FollowBox(string boxid)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new SubscribeToSharedBoxCommand(userid, boxId);
                m_ZboxWriteService.SubscribeToSharedBox(command);
                return ResultDto<string>.GetSuccessResult(string.Empty);
            }
            catch (ArgumentException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Gone, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api FollowBox , userid {0} boxid {1}", userid, boxid), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Invite someone to a box")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{boxid}/Invite")]
        public ResultDto<string> Invite(string boxid, InviteToBox model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            if (model.EmailList.Where(w => !string.IsNullOrWhiteSpace(w)).Count() == 0)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "You need at least invite with one email");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new ShareBoxCommand(boxId, userid, model.EmailList, model.PersonalMessage);
                m_ZboxWriteService.ShareBox(command);
                return ResultDto<string>.GetSuccessResult(string.Empty);
            }
            catch (ArgumentException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Gone, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api Invite , userid {0} boxid {1} model {2}", userid, boxid, model), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Request a url to upload to a specific blob version 2")]
        [WebInvoke(Method = "POST", UriTemplate = "Boxes/{boxid}/Files")]
        public ResultDto<string> GetBlobUploadPermissionV2(string boxid, CreateFile model)
        {
            return GetBlobUploadPermission(boxid, model.BlobName);
        }

        [Description("Add file to a box")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{boxid}/Files")]
        public ResultDto<FileDto> AddFileToBox(string boxid, AddFileToBox model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<FileDto>(HttpStatusCode.Unauthorized, "Need to authenticate to upload file");
            }
            try
            {
                var blob = m_BlobProvider.GetFile(model.BlobName);
                if (!blob.Exists())
                {
                    return WriteOutgoingErrorResponse<FileDto>(HttpStatusCode.BadRequest, "blob doesn't exists");
                }
                if (blob.Properties.Length == 0)
                {
                    return WriteOutgoingErrorResponse<FileDto>(HttpStatusCode.BadRequest, "blob should be with data");
                }
                //if (userid.ToString() != blob.Metadata[BlobProvider.blobMetadataUseridKey])
                //{
                //    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "You did not upload this data");
                //}
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new AddFileToBoxCommand(userid, boxId, model.BlobName, model.FileName, blob.Properties.Length, Guid.NewGuid().ToString());
                var result = m_ZboxWriteService.AddFileToBox(command);
                var FileDto = new FileDto(result.File.Uid, result.File.Name, result.File.DateTimeUser.CreationTime,
                    result.File.DateTimeUser.UpdateTime, result.File.BlobName, result.File.ContentUpdateTime.Value,
                     result.File.ThumbnailBlobName,
                     0, result.File.UploaderId.Name, result.File.UploaderId.Uid, result.File.UploaderId.Image, result.File.UploaderId.UserType);
                return ResultDto<FileDto>.GetSuccessResult(FileDto);
            }
            catch (FileQuotaExceedException)
            {
                return WriteOutgoingErrorResponse<FileDto>(HttpStatusCode.BadRequest, "User exceeds his quota limit");

            }
            catch (ArgumentException ex)
            {
                TraceLog.WriteError(string.Format("Rest api AddFileToBox , userid {0} boxid {1} data {2}", userid, boxid, model), ex);
                return WriteOutgoingErrorResponse<FileDto>(HttpStatusCode.BadRequest, ex.Message);

            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api AddFileToBox , userid {0} boxid {1} data {2}", userid, boxid, model), ex);
                return WriteOutgoingErrorResponse<FileDto>(HttpStatusCode.InternalServerError, "");
            }

        }

        [Description("Update file content")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{Boxid}/Items/{itemid}")]
        public ResultDto<string> UpdateFile(string boxid, string itemid, AddFileToBox model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to update file");
            }
            try
            {
                var blob = m_BlobProvider.GetFile(model.BlobName);
                if (!blob.Exists())
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "blob doesn't exists");
                }
                if (blob.Properties.Length == 0)
                {
                    return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "blob should be with data");
                }

                //var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, Infrastructure.ShortUrl.ShortCodesType.Item);
                var command = new UpdateFileCommand(userid, itemId, model.BlobName);
                m_ZboxWriteService.UpdateFile(command);

                return ResultDto<string>.GetSuccessResult(string.Empty);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api UpdateFile , userid {0} boxid {1} data {2}, itemid {3}", userid, boxid, model, itemid), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Create new box")]
        [WebInvoke(Method = "POST", UriTemplate = "Boxes")]
        public ResultDto<ZboxDto.BoxWithDetailDto> CreateBox(CreateBox model)
        {
            if (!Enum.IsDefined(typeof(NotificationSettings), model.NotificationSettings))
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.BadRequest, "Notification setting not in correct value");
            }
            if (!Enum.IsDefined(typeof(BoxPrivacySettings), model.BoxPrivacySettings))
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.BadRequest, "BoxPrivacySettings not in correct value");
            }
            if (!Validation.IsValidFileName(model.BoxName))
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.BadRequest, "Box name is not a valid name");
            }
            if (model.BoxName.Length > Box.NameLength)
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.BadRequest, "Box name should not exceed " + Box.NameLength);
            }
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }

            try
            {
                var command = new CreateBoxCommand(userid, model.BoxName, new string[0]);
                var result = m_ZboxWriteService.CreateBox(command);
                ZboxDto.BoxWithDetailDto dto = new ZboxDto.BoxWithDetailDto
                {
                    Name = result.NewBox.Name,
                    MembersCount = 0,
                    Uid = result.NewBox.Uid,
                    PrivacySetting = result.NewBox.PrivacySettings.PrivacySetting
                    //Owner = result.NewBox.GetUserOwner().Name
                };
                var itemTemplate = WebOperationContext.Current.GetUriTemplate("GetBox");
                var uri = itemTemplate.BindByPosition(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri, dto.Uid);
                WebOperationContext.Current.OutgoingResponse.SetStatusAsCreated(uri);
                return ResultDto<ZboxDto.BoxWithDetailDto>.GetSuccessResult(dto);

            }
            catch (BoxNameAlreadyExistsException)
            {
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.Conflict, "Box name already exists");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api CreateBox , userid {0} data {1}",
                    userid, model), ex);
                return WriteOutgoingErrorResponse<ZboxDto.BoxWithDetailDto>(HttpStatusCode.InternalServerError, "");

            }
        }

        [Description("Change name of existing box")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{boxid}/ChangeName")]
        public ResultDto<string> ChangeBoxName(string boxid, ChangeBoxName model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            if (!Validation.IsValidFileName(model.NewBoxName))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "New box name is not a valid name");
            }
            if (model.NewBoxName.Length > Box.NameLength)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "Box name should not exceed " + Box.NameLength);
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new ChangeBoxNameCommand(boxId, userid, model.NewBoxName);
                m_ZboxWriteService.ChangeBoxName(command);
                return ResultDto<string>.GetSuccessResult(model.NewBoxName);
            }
            catch (ArgumentNullException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api ChangeBoxName , userid {0} boxid {1} data {2}", userid, boxid, model), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Change name of existing item")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{boxid}/Items/{itemid}/ChangeName")]
        public ResultDto<string> ChangeItemName(string boxid, string itemId, ChangeFileName model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            if (!Validation.IsValidFileName(model.NewFileName))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "New item name is not a valid name");
            }
            try
            {
                var itemid = m_ShortToLongCode.ShortCodeToLong(itemId, ShortCodesType.Item);
                var command = new ChangeFileNameCommand(itemid, model.NewFileName, userid);
                var result = m_ZboxWriteService.ChangeFileName(command);
                return ResultDto<string>.GetSuccessResult(result.File.Name);
            }
            catch (ArgumentNullException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api ChangeItemName , userid {0} boxid {1} itemid{2} data {3}", userid, boxid, itemId, model), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Change box privacy settings of existing box")]
        [WebInvoke(Method = "PUT", UriTemplate = "Boxes/{boxid}/PrivacySettings")]
        public ResultDto<string> ChangeBoxPrivacySettings(string boxid, ChangeBoxPrivacySettings model)
        {
            if (!Enum.IsDefined(typeof(BoxPrivacySettings), model.NewBoxPrivacySettings))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "BoxPrivacySettings does not have the right value");
            }
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new ChangeBoxPrivacySettingsCommand(userid, boxId, model.NewBoxPrivacySettings, "");
                var result = m_ZboxWriteService.ChangeBoxPrivacySettings(command);
                return ResultDto<string>.GetSuccessResult(model.NewBoxPrivacySettings.GetStringValue());
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api ChangeBoxPrivacySettings , userid {0} boxid {1} data {2}", userid, boxid, model), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }





        [Description("Add comment to box")]
        [WebInvoke(Method = "POST", UriTemplate = "Boxes/{boxid}/Comments")]
        public ResultDto<ZboxDto.CommentDto> AddBoxComment(string boxid, AddComment model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<ZboxDto.CommentDto>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new AddBoxCommentCommand(userid, boxId, model.Comment);
                var result = m_ZboxWriteService.AddBoxComment(command);
                ZboxDto.CommentDto commentDto = new ZboxDto.CommentDto
                {
                    AuthorName = result.User.Name,
                    UserImage = result.User.Image,
                    CreationTime = DateTime.SpecifyKind(result.NewComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
                    CommentText = result.NewComment.CommentText,
                    Id = result.NewComment.Id,
                    BoxUid = result.Target.Uid
                };
                return ResultDto<ZboxDto.CommentDto>.GetSuccessResult(commentDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<ZboxDto.CommentDto>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api AddBoxComment , userid {0} boxid {1} Comment {2}", userid, boxid, model.Comment), ex);
                return WriteOutgoingErrorResponse<ZboxDto.CommentDto>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Add comment to item")]
        [WebInvoke(Method = "POST", UriTemplate = "Boxes/{boxid}/Items/{itemid}/Comments")]
        public ResultDto<ZboxDto.CommentDto> AddItemComment(string boxid, string itemid, AddComment model)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<ZboxDto.CommentDto>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
                var command = new AddItemCommentCommand(userid, itemId, boxId, model.Comment);
                var result = m_ZboxWriteService.AddItemComment(command);
                ZboxDto.CommentDto commentDto = new ZboxDto.CommentDto
                {
                    AuthorName = result.User.Name,
                    CreationTime = DateTime.SpecifyKind(result.ItemComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
                    UpdateTime = DateTime.SpecifyKind(result.ItemComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
                    CommentText = result.ItemComment.CommentText,
                    UserImage = result.User.Image,
                    Id = result.ItemComment.Id,
                    BoxUid = boxid,
                    ItemUid = itemid
                };
                return ResultDto<ZboxDto.CommentDto>.GetSuccessResult(commentDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<ZboxDto.CommentDto>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api AddItemComment , userid {0} boxid {1} Comment {2} itemid {3}", userid, boxid, model.Comment, itemid), ex);
                return WriteOutgoingErrorResponse<ZboxDto.CommentDto>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Delete box item (file/url)")]
        [WebInvoke(Method = "DELETE", UriTemplate = "Boxes/{boxid}/Items/{itemid}")]
        public ResultDto<string> DeleteItem(string boxid, string itemid)
        {
            var userid = GetUserId();
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
                var command = new DeleteItemCommand(itemId, userid, boxId);
                m_ZboxWriteService.DeleteItem(command);
                return ResultDto<string>.GetSuccessResult(string.Empty);

            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api DeleteItem , userid {0} boxid {1} itemid {2}", userid, boxid, itemid), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }

        [Description("Delete comment from item or box")]
        [WebInvoke(Method = "DELETE", UriTemplate = "Boxes/{boxid}/Comments/{commentid}")]
        public ResultDto<string> DeleteComment(string boxid, string commentid)
        {
            var userid = GetUserId();
            long commentId = 0;
            if (!long.TryParse(commentid, out commentId))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, "commentid should be interger");

            }
            if (!IsAuthorized(userid))
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Unauthorized, "Need to authenticate to change data");
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new DeleteCommentCommand(commentId, userid, boxId);
                m_ZboxWriteService.DeleteComment(command);
                return ResultDto<string>.GetSuccessResult(string.Empty);

            }
            catch (ArgumentException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Rest api DeleteComment , userid {0} boxid {1} commentid {2}", userid, boxid, commentid), ex);
                return WriteOutgoingErrorResponse<string>(HttpStatusCode.InternalServerError, "");
            }
        }
    }
}