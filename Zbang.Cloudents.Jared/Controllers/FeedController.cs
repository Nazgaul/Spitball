using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.ViewModel.Queries.QnA;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    [Authorize]
    public class FeedController : ApiController
    {
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IZboxReadService m_ZboxReadService;

        public FeedController(IGuidIdGenerator guidGenerator, IZboxWriteService zboxWriteService, IZboxReadService zboxReadService)
        {
            m_GuidGenerator = guidGenerator;
            m_ZboxWriteService = zboxWriteService;
            m_ZboxReadService = zboxReadService;
        }

        [HttpPost]
        [Authorize]
        [Route("api/course/{boxId:long}/feed")]
        public async Task<HttpResponseMessage> PostCommentAsync(long boxId, AddCommentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }

            var questionId = m_GuidGenerator.GetId();
            var command = new AddCommentCommand(User.GetUserId(),
                boxId, model.Content, questionId, model.FilesIds, model.Anonymous);
            var result = await m_ZboxWriteService.AddCommentAsync(command).ConfigureAwait(false);

            if (model.Tags.Any())
            {
                var z = new AssignTagsToFeedCommand(result.CommentId, model.Tags, TagType.User);
                await m_ZboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet, Route("api/course/{boxId:long}/feed/{feedId:guid}")]
        public async Task<HttpResponseMessage> GetFeedAsync(long boxId, Guid feedId)
        {
            var retVal =
                await m_ZboxReadService.GetCommentAsync(new GetQuestionQuery(feedId, boxId)).ConfigureAwait(false);

            return Request.CreateResponse(new
            {
                Files = retVal.Files.Select(s => new
                {
                    s.Type,
                    s.Source,
                    s.Id,
                    s.Name

                }),
                retVal.UserImage,
                retVal.UserName,
                retVal.Content,
                retVal.CreationTime,
                retVal.LikesCount,
                retVal.Id
            });
        }

        [HttpGet, Route("api/course/{boxId:long}/feed/{feedId:guid}/replies")]
        public async Task<HttpResponseMessage> GetRepliesAsync(long boxId, Guid feedId, string belowReplyId, int page, int sizePerPage = 20)
        {
            Guid replyId;

            if (!Guid.TryParse(belowReplyId, out replyId))
            {
                replyId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
            }

            //var replyId = belowReplyId ?? Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
            var retVal =
                 await m_ZboxReadService.GetRepliesAsync(new GetCommentRepliesQuery(boxId, feedId, replyId, page, sizePerPage)).ConfigureAwait(false);
            return Request.CreateResponse(retVal.Select(s => new
            {
                s.Id,
                s.UserImage,
                s.UserName, 
                s.Content,
                s.CreationTime,
                s.LikesCount,
                Files = s.Files.Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Type,
                    d.Source
                })


            }));
        }

        [Authorize, HttpPost, Route("api/course/{boxId:long}/feed/{id:guid}/like")]
        public async Task<HttpResponseMessage> LikeCommentAsync(Guid id, long boxId, [FromBody] TagsRequest model)
        {
            var command = new LikeCommentCommand(id, User.GetUserId(), boxId);
            await m_ZboxWriteService.LikeCommentAsync(command).ConfigureAwait(false);

            if (model?.Tags != null && model.Tags.Any())
            {
                var z = new AssignTagsToFeedCommand(id, model.Tags, TagType.User);
                await m_ZboxWriteService.AddItemTagAsync(z).ConfigureAwait(false);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
            //return JsonOk(retVal.Liked);
        }

        [Authorize, HttpPost, Route("api/course/{boxId:long}/feed/{commentId:guid}/reply/{replyId:guid}/like")]
        public async Task<HttpResponseMessage> LikeReplyAsync(Guid replyId, long boxId)
        {
            var command = new LikeReplyCommand(replyId, User.GetUserId(), boxId);
            await m_ZboxWriteService.LikeReplyAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [Authorize, HttpPost]
        [Route("api/course/{boxId:long}/feed/{feedId:guid}/reply")]
        public async Task<HttpResponseMessage> PostReplyAsync(long boxId, Guid feedId, AddCommentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var answerId = m_GuidGenerator.GetId();
            var command = new AddReplyToCommentCommand(User.GetUserId(), boxId,
                model.Content, answerId, feedId, model.FilesIds);
            await m_ZboxWriteService.AddReplyAsync(command).ConfigureAwait(false);
            return Request.CreateResponse(answerId);
        }
    }
}
