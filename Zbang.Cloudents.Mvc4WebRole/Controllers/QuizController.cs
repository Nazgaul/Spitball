using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Quiz;
using Zbang.Cloudents.Mvc4WebRole.Extensions;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.Mvc4WebRole.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    [ZboxAuthorize]

    public class QuizController : BaseController
    {
        private readonly Lazy<IIdGenerator> m_IdGenerator;


        public QuizController(IZboxWriteService zboxWriteService,
            IZboxReadService zboxReadService,
            IFormsAuthenticationService formsAuthenticationService,
            Lazy<IIdGenerator> idGenerator)
            : base(zboxWriteService, zboxReadService,
            formsAuthenticationService)
        {
            m_IdGenerator = idGenerator;
        }
        //
        // GET: /Quiz/
        public ActionResult Index()
        {
            return View();
        }

        #region Quiz
        [HttpPost, Ajax]
        public ActionResult Create(Quiz model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }

            return this.CdJson(new JsonResponse(true));
        }

        [HttpPost, Ajax]
        public ActionResult Update(UpdateQuiz model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }

            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult Delete(long id)
        {
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult Save(long id)
        {
            return this.CdJson(new JsonResponse(true));
        }
        #endregion

        #region question
        [HttpPost, Ajax]
        public ActionResult CreateQuestion(Question model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult UpdateQuestion(UpdateQuestion model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }

            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult DeleteQuestion(Guid id)
        {
            return this.CdJson(new JsonResponse(true));
        }
        #endregion

        #region Answer
        [HttpPost, Ajax]
        public ActionResult CreateAnswer(Answer model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }
            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult UpdateAnswer(UpdateAnswer model)
        {
            if (!ModelState.IsValid)
            {
                return this.CdJson(new JsonResponse(false, GetErrorsFromModelState()));
            }

            return this.CdJson(new JsonResponse(true));
        }
        [HttpPost, Ajax]
        public ActionResult DeleteAnswer(Guid id)
        {
            return this.CdJson(new JsonResponse(true));
        }
        #endregion
    }
}