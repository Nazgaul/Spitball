using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Cloudents.Mvc4WebRole.Models.QnA;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]

    public class QnAController : BaseController
    {
        private readonly Lazy<IIdGenerator> m_IdGenerator;
        public QnAController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService,
            Lazy<IIdGenerator> idGenerator)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_IdGenerator = idGenerator;
        }


        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult AddQuestion(Question model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            //  var filesId = model.Files.Select(s => m_ShortToLongCode.ShortCodeToLong(s, ShortCodesType.Item));
            var questionId = m_IdGenerator.Value.GetId();
            var command = new AddCommentCommand(GetUserId(), model.BoxUid, model.Content, questionId, model.Files);
            m_ZboxWriteService.AddQuestion(command);
            return Json(new JsonResponse(true, questionId));
        }

        [ZboxAuthorize]
        [Ajax, HttpPost]
        public async Task<JsonResult> AddAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new JsonResponse(false, GetErrorsFromModelState()));
            }
            var answerId = m_IdGenerator.Value.GetId();
            var command = new AddAnswerToQuestionCommand(GetUserId(), model.BoxUid, model.Content, answerId, model.QuestionId, model.Files);
            await m_ZboxWriteService.AddAnswer(command);
            return Json(new JsonResponse(true, answerId));
        }
        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult MarkAnswer(Guid answerId)
        {
            var command = new MarkAsAnswerCommand(answerId, GetUserId());
            m_ZboxWriteService.MarkCorrectAnswer(command);
            return Json(new JsonResponse(true));
        }
        //[ZboxAuthorize]
        //[Ajax, HttpPost]
        //public JsonResult RateQuestion(Guid answerId)
        //{
        //    var rateId = m_IdGenerator.Value.GetId();
        //    var command = new RateAnswerCommand(GetUserId(), answerId, rateId);
        //    m_ZboxWriteService.RateAnswer(command);
        //    return Json(new JsonResponse(true));
        //}

        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult RemoveFile(long itemId)
        {
            var command = new DeleteFileFromQnACommand(itemId, GetUserId());
            m_ZboxWriteService.DeleteFileFromQnA(command);
            return Json(new JsonResponse(true));
        }

        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult DeleteQuestion(Guid questionId)
        {
            try
            {
                var command = new DeleteCommentCommand(questionId, GetUserId());
                m_ZboxWriteService.DeleteComment(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete question questionId {0} userid {1}", questionId.ToString(), GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }
        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult DeleteAnswer(Guid answerId)
        {
            try
            {
                var command = new DeleteReplyCommand(answerId, GetUserId());
                m_ZboxWriteService.DeleteAnswer(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete answer answerId {0} userid {1}", answerId.ToString(), GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }


        [Ajax, HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        [AjaxCache(TimeToCache = TimeConsts.Minute * 20)]
        public ActionResult Index(long boxId, string uniName, string boxName)
        {
            //this is a test
            var retVal = m_ZboxReadService.GetQuestions(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, GetUserId(false)));
            var urlBuilder = new UrlBuilder(HttpContext);
            retVal.ToList().ForEach(f =>
            {
                f.Url = urlBuilder.BuildUserUrl(f.UserUid, f.UserName);
                f.Files.ForEach(fi => fi.Url = urlBuilder.buildItemUrl(boxId, boxName, fi.Uid, fi.Name, uniName));

                f.Answers.ForEach(fa => fa.Files.ForEach(fi1 => fi1.Url = urlBuilder.buildItemUrl(boxId, boxName, fi1.Uid, fi1.Name, uniName)));
                f.Answers.ForEach(fa => fa.Url = urlBuilder.BuildUserUrl(fa.UserId, fa.UserName));
            });
            return this.CdJson(new JsonResponse(true, retVal));
        }



    }
}
