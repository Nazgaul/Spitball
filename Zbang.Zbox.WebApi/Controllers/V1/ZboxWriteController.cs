using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Zbang.Zbox.ApiViewModel.Queries;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WebApi.Helpers;
using Zbang.Zbox.WebApi.Models;
using Zbang.Zbox.Infrastructure.Storage;
using ZboxDto = Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ApiViewModel.DTOs;
using Zbang.Zbox.Infrastructure.Url;

namespace Zbang.Zbox.WebApi.Controllers
{
    public partial class ZboxController
    {
        //[Description("Delete or unfollow specific box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}")]
        [ZboxAuthorize(IsAuthenticationRequired = true)]
        [ActionName("Box")]
        [HttpDelete]
        public HttpResponseMessage DeleteBox(string boxid)
        {
            var userid = GetUserId();

            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var userboxes = m_ZboxReadService.GetBoxes(new GetBoxesQuery(userid));
                var box = userboxes.FirstOrDefault(w => w.Uid == boxid);
                if (box.MemberType == Infrastructure.Enums.UserRelationshipType.Owner)
                {
                    var command = new DeleteBoxCommand(boxId, userid);
                    m_ZboxWriteService.DeleteBox(command);
                    return Request.CreateZboxOkResult(string.Empty);
                }
                if (box.MemberType == Infrastructure.Enums.UserRelationshipType.Subscribe)
                {
                    var command = new DeleteUserFromBoxCommand(userid, userid, boxId);
                    m_ZboxWriteService.DeleteUserFromBox(command);
                    return Request.CreateZboxOkResult(string.Empty);
                }

                TraceLog.WriteError(string.Format("Rest api DeleteBox , userid {0} boxid {1} reach to not implemented", userid, boxid));
                return Request.CreateZboxErrorResponse(HttpStatusCode.NotImplemented, string.Empty);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }
            catch (BoxDoesntExistException)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Gone, "Box doesn't exists");
            }
            catch (InvalidOperationException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }
        }

        //[Description("follow specific box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Follow")]
        [HttpPut]
        [ZboxAuthorize]
        public HttpResponseMessage FollowBox(string boxid)
        {
            var userid = GetUserId();

            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new SubscribeToSharedBoxCommand(userid, boxId);
                m_ZboxWriteService.SubscribeToSharedBox(command);
                return Request.CreateZboxOkResult(string.Empty);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Gone, ex.Message);
            }
        }

        //[Description("Invite someone to a box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Invite")]
        [HttpPut]
        [ZboxAuthorize]
        public HttpResponseMessage Invite(string boxid, [FromBody] InviteToBox model)
        {
            var userid = GetUserId();
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }

            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new ShareBoxCommand(boxId, userid, model.EmailList);
                m_ZboxWriteService.ShareBox(command);
                return Request.CreateZboxOkResult(string.Empty);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Gone, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }
        }

        //[Description("Request a url to upload to a specific blob version 2")]
        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}/Files")]
        //[ZboxAuthorize]
        //[HttpPost]
        //[ActionName("FileToBox")]
        //public HttpResponseMessage PostFileToBox(string boxid, [FromBody] CreateFile model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateZboxErrorResponse(ModelState);
        //    }
        //    return GetBlobUploadPermission(boxid, model.BlobName);
        //}

        //[Description("Add file to a box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Files")]
        [ZboxAuthorize]
        [HttpPut]
        [ActionName("FileToBox")]
        public HttpResponseMessage PutFileToBox(string boxid, [FromBody] AddFileToBox model)
        {
            var userid = GetUserId();
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }
            try
            {
                var blob = m_BlobProvider.GetFile(model.BlobName);
                if (!blob.Exists())
                {
                    return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "blob doesn't exists");
                }
                if (blob.Properties.Length == 0)
                {
                    return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "blob should be with data");
                }

                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new AddFileToBoxCommand(userid, boxId, model.BlobName, model.FileName, blob.Properties.Length, null);
                var result = m_ZboxWriteService.AddFileToBox(command);
                var FileDto = new FileDto(result.File.Uid, result.File.Name, result.File.DateTimeUser.CreationTime,
                    result.File.DateTimeUser.UpdateTime, result.File.ItemContentUrl, result.File.ContentUpdateTime.Value,
                     result.File.ThumbnailBlobName,
                     0, result.File.UploaderId.Name, result.File.UploaderId.Id, result.File.UploaderId.Image, Zbang.Zbox.Infrastructure.Enums.UserType.Regular);
                return Request.CreateZboxOkResult(FileDto);
            }
            catch (FileQuotaExceedException)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "User exceeds his quota limit");

            }
            catch (ArgumentException ex)
            {
                TraceLog.WriteError(string.Format("Rest api AddFileToBox , userid {0} boxid {1} data {2}", userid, boxid, model), ex);
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, ex.Message);

            }
        }

        // [Description("Update file content")]
        [RoutingAttribute(UriTemplate = "Boxes/{Boxid}/Items/{itemid}")]
        [ZboxAuthorize]
        [HttpPut]
        public HttpResponseMessage UpdateFile(string boxid, string itemid, [FromBody] AddFileToBox model)
        {
            var userid = GetUserId();
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }

            var blob = m_BlobProvider.GetFile(model.BlobName);
            if (!blob.Exists())
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "blob doesn't exists");
            }
            if (blob.Properties.Length == 0)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "blob should be with data");
            }

            //var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
            var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, Infrastructure.Url.ShortCodesType.Item);
            var command = new UpdateFileCommand(userid, itemId, model.BlobName);
            m_ZboxWriteService.UpdateFile(command);

            return Request.CreateZboxOkResult(string.Empty);


        }

        //[Description("Create new box")]
        //TODO fix create box to recive tags
        [RoutingAttribute(UriTemplate = "Boxes")]
        [ZboxAuthorize]
        [HttpPost]
        public HttpResponseMessage CreateBox([FromBody] CreateBox model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }

            var userid = GetUserId();
            //try
            //{
            //var command = new CreateBoxCommand(userid, model.BoxName, model.NotificationSettings, model.BoxPrivacySettings);
            var command = new CreateBoxCommand(userid, model.BoxName, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var result = m_ZboxWriteService.CreateBox(command);
            ZboxDto.BoxWithDetailDto dto = new ZboxDto.BoxWithDetailDto
            {
                Name = result.NewBox.Name,
                MembersCount = 0,
                Uid = result.NewBox.Uid,
                PrivacySetting = result.NewBox.PrivacySettings.PrivacySetting
                //Owner = result.NewBox.GetUserOwner().Name
            };
            var retVal = Request.CreateZboxOkResult(dto, HttpStatusCode.Created);
            retVal.Headers.Location = new Uri(Url.Link("GetBox", new { boxid = dto.Uid }));
            return retVal;
            //}
            //catch (BoxNameAlreadyExistsException)
            //{
            //    return Request.CreateZboxErrorResponse(HttpStatusCode.Conflict, "Box name already exists");
            //}
        }

        //[Description("Change name of existing box")]
        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}/ChangeName")]
        //[ZboxAuthorize]
        //[HttpPut]
        //public HttpResponseMessage ChangeBoxName(string boxid, [FromBody] ChangeBoxName model)
        //{
        //    var userid = GetUserId();
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateZboxErrorResponse(ModelState);
        //    }
        //    try
        //    {
        //        var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
        //        var command = new ChangeBoxInfoCommand(boxId, userid, model.NewBoxName, null, null);
        //        m_ZboxWriteService.ChangeBoxInfo(command);
        //        return Request.CreateZboxOkResult(model.NewBoxName);
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
        //    }

        //}

        //[Description("Change name of existing item")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items/{itemid}/ChangeName")]
        [ZboxAuthorize]
        [HttpPut]
        public HttpResponseMessage ChangeItemName(string boxid, string itemId, [FromBody] ChangeFileName model)
        {
            var userid = GetUserId();
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }

            try
            {
                var itemid = m_ShortToLongCode.ShortCodeToLong(itemId, ShortCodesType.Item);
                var command = new ChangeFileNameCommand(itemid, model.NewFileName, userid);
                var result = m_ZboxWriteService.ChangeFileName(command);
                return Request.CreateZboxOkResult(result.File.Name);
            }
            catch (ArgumentNullException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }

        }

        //[Description("Change box privacy settings of existing box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/PrivacySettings")]
        [ZboxAuthorize]
        [HttpPut]
        public HttpResponseMessage ChangeBoxPrivacySettings(string boxid, [FromBody] ChangeBoxPrivacySettings model)
        {

            var userid = GetUserId();
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
            }
            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new ChangeBoxPrivacySettingsCommand(userid, boxId, model.NewBoxPrivacySettings, "");
                var result = m_ZboxWriteService.ChangeBoxPrivacySettings(command);
                return Request.CreateZboxOkResult(model.NewBoxPrivacySettings.GetStringValue());
            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }

        }





        //[Description("Add comment to box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Comments")]
        [ZboxAuthorize]
        [HttpPost]
        public HttpResponseMessage AddBoxComment(string boxid, [FromBody] AddComment model)
        {
            var userid = GetUserId();
            if (!ModelState.IsValid)
            {
                return Request.CreateZboxErrorResponse(ModelState);
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
                return Request.CreateZboxOkResult(commentDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }

        }

        //[Description("Add comment to item")]
        //[RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items/{itemid}/Comments")]
        //[ZboxAuthorize]
        //[ActionName("ItemComments")]
        //[HttpPost]
        //public HttpResponseMessage AddItemComment(string boxid, string itemid, [FromBody] AddComment model)
        //{
        //    var userid = GetUserId();

        //    try
        //    {
        //        var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
        //        var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
        //        var command = new AddItemCommentCommand(userid, itemId, boxId, model.Comment);
        //        var result = m_ZboxWriteService.AddItemComment(command);
        //        ZboxDto.CommentDto commentDto = new ZboxDto.CommentDto
        //        {
        //            AuthorName = result.User.Name,
        //            CreationTime = DateTime.SpecifyKind(result.ItemComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
        //            UpdateTime = DateTime.SpecifyKind(result.ItemComment.DateTimeUser.CreationTime, DateTimeKind.Unspecified),
        //            CommentText = result.ItemComment.CommentText,
        //            UserImage = result.User.Image,
        //            Id = result.ItemComment.Id,
        //            BoxUid = boxid,
        //            ItemUid = itemid
        //        };
        //        return Request.CreateZboxOkResult(commentDto);
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
        //    }

        //}

        //[Description("Delete box item (file/url)")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Items/{itemid}")]
        [ZboxAuthorize]
        [ActionName("Item")]
        [HttpDelete]
        public HttpResponseMessage DeleteItem(string boxid, string itemid)
        {
            var userid = GetUserId();

            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var itemId = m_ShortToLongCode.ShortCodeToLong(itemid, ShortCodesType.Item);
                var command = new DeleteItemCommand(itemId, userid, boxId);
                m_ZboxWriteService.DeleteItem(command);
                return Request.CreateZboxOkResult(string.Empty);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }
        }

        //[Description("Delete comment from item or box")]
        [RoutingAttribute(UriTemplate = "Boxes/{boxid}/Comments/{commentid}")]
        [ZboxAuthorize]
        [HttpDelete]
        public HttpResponseMessage DeleteComment(string boxid, string commentid)
        {
            var userid = GetUserId();
            long commentId = 0;
            if (!long.TryParse(commentid, out commentId))
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, "commentid should be interger");

            }

            try
            {
                var boxId = m_ShortToLongCode.ShortCodeToLong(boxid);
                var command = new DeleteCommentCommand(commentId, userid, boxId);
                m_ZboxWriteService.DeleteComment(command);
                return Request.CreateZboxOkResult(string.Empty);

            }
            catch (ArgumentException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Request.CreateZboxErrorResponse(HttpStatusCode.Forbidden, ex.Message);
            }

        }
    }
}