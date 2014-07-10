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
            ZboxWriteService.AddQuestion(command);
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
            await ZboxWriteService.AddAnswer(command);
            return Json(new JsonResponse(true, answerId));
        }
        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult MarkAnswer(Guid answerId)
        {
            var command = new MarkAsAnswerCommand(answerId, GetUserId());
            ZboxWriteService.MarkCorrectAnswer(command);
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
            ZboxWriteService.DeleteFileFromQnA(command);
            return Json(new JsonResponse(true));
        }

        [ZboxAuthorize]
        [Ajax, HttpPost]
        public JsonResult DeleteQuestion(Guid questionId)
        {
            try
            {
                var command = new DeleteCommentCommand(questionId, GetUserId());
                ZboxWriteService.DeleteComment(command);
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
                ZboxWriteService.DeleteAnswer(command);
                return Json(new JsonResponse(true));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError(string.Format("Delete answer answerId {0} userid {1}", answerId, GetUserId()), ex);
                return Json(new JsonResponse(false));
            }
        }


        [Ajax, HttpGet, ZboxAuthorize(IsAuthenticationRequired = false)]
        //[AjaxCache(TimeToCache = TimeConsts.Minute * 20)]
        public ActionResult Index(long boxId, string uniName, string boxName)
        {
            //this is a test
            var retVal = ZboxReadService.GetQuestions(new Zbox.ViewModel.Queries.QnA.GetBoxQuestionsQuery(boxId, GetUserId(false)));
            return this.CdJson(new JsonResponse(true, retVal));
        }



    }
}
