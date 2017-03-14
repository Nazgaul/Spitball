using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Cloudents.Mvc2Jared.Models;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Queries;
using System.Threading;
using Zbang.Zbox.Infrastructure.Search;
using Newtonsoft.Json;
using Zbang.Zbox.ViewModel.Queries.Search;
using Zbang.Zbox.ViewModel.Dto.Search;
using System.Collections.Generic;

namespace Zbang.Cloudents.Mvc2Jared.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.Disabled)]
    public class HomeController : BaseController
    {
       // private readonly IZboxReadService m;
        //private readonly IItemReadSearchProvider m_ItemSearchService;
        private readonly Lazy<ApplicationUserManager> m_UserManager;
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Page()
        {
            return View();
        }
        [HttpPost, ActionName("Items")]
        public async Task<JsonResult> ItemsAsync(SearchItemWatson model, CancellationToken cancellationToken)
        {
            int a;
           // var retVal = await m.
            //SearchItemWatson search = JsonConvert.DeserializeObject<SearchItemWatson>(term);
            a = 5;
            //return Json(retVal);
            return Json("");
            //return JsonOk();
            //return await SearchAllQueryAsync(cancellationToken,search);
        }
        //public async Task<IEnumerable<SearchDocument>> SearchItemAsync(SearchItemWatson query, CancellationToken cancelToken)
        //{
            //if (query == null) throw new ArgumentNullException(nameof(query));
          //  var term = query.Term;
            //if we put asterisk highlight is not working
            //http://stackoverflow.com/questions/35769442/azure-search-highlights-not-returned-for-every-result/35778095
            //if (!query.Term.Contains(" "))
            //{
            //    term += "*";
            //}
            //var filter = await m_FilterProvider.BuildFilterExpressionAsync(
            //   query.UniversityId, UniversityidField, UserIdsField, query.UserId);

            //var result = await m_IndexClient.Documents.SearchAsync<ItemSearch>(term, new SearchParameters
            //{
            //    Filter = filter,
            //    Top = query.RowsPerPage,
            //    Skip = query.RowsPerPage * query.PageNumber,
            //    ScoringProfile = ScoringProfileName,
            //    ScoringParameters = new[] { new ScoringParameter("university", new[] { query.UniversityId.ToString() }) },
            //    Select = new[] { BoxNameField, SmallContentField, IdField, ImageField, NameField, UniversityNameField, UrlField, BlobNameField },
            //    HighlightFields = new[] { ContentField, NameField }
            //}, cancellationToken: cancelToken);

            //return result.Results.Select(s => new SearchDocument
            //{
            //    Boxname = s.Document.BoxName,
            //    Content = HighLightInField(s, ContentField, s.Document.MetaContent),
            //    Id = long.Parse(s.Document.Id),
            //    //Image = s.Document.Image,
            //    Name = HighLightInField(s, NameField, s.Document.Name),
            //    UniName = s.Document.UniversityName,
            //    Url = s.Document.Url,
            //    Source = s.Document.BlobName
            //});
        //}
        private async Task<JsonResult> SearchAllQueryAsync(CancellationToken cancellationToken,
          SearchItemWatson query)
        {
            try
            {
                using (var source = CreateCancellationToken(cancellationToken))
                {
                    //var retVal = await SearchItemAsync(query, source.Token);
                    return JsonOk();
                }
            }
            catch (OperationCanceledException)
            {
                return JsonOk();
            }
        }
        [HttpPost]
        [ActionName("Login")]
        public async Task<JsonResult> LogInAsync(LogOn model)
        {
            if (!ModelState.IsValid)
            {
                return JsonError(GetErrorFromModelState());
            }
            try
            {
                if (model.Email == "yifatbij@gmail.com" && model.Password == "123123")
                {
                    return JsonOk(Url.Action("Page", "home"));
                }
                //ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidPassword));
                return JsonError(new { error="invalid password Or user name"});
                //var query = new GetUserByEmailQuery(model.Email);
                //var tSystemData = ZboxReadService.GetUserDetailsByEmail(query);
                //var tUserIdentity = m_UserManager.Value.FindByEmailAsync(model.Email);

                //await Task.WhenAll(tSystemData, tUserIdentity);

                //var user = tUserIdentity.Result;
                //var systemUser = tSystemData.Result;


                //if (systemUser == null)
                //{
                //    ModelState.AddModelError(string.Empty, AccountControllerResources.LogonError);
                //    return JsonError(GetErrorFromModelState());
                //}

                //if (systemUser.MembershipId.HasValue)
                //{
                //    if (user == null)
                //    {
                //        ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidEmail));
                //        return JsonError(GetErrorFromModelState());
                //    }
                //    var loginStatus = await m_UserManager.Value.CheckPasswordAsync(user, model.Password);

                //    if (loginStatus)
                //    {
                //        var identity = await user.GenerateUserIdentityAsync(m_UserManager.Value, systemUser.Id,
                //            systemUser.UniversityId, systemUser.UniversityData);
                //        m_AuthenticationManager.SignIn(new AuthenticationProperties
                //        {
                //            IsPersistent = model.RememberMe,
                //        }, identity);

                //        m_CookieHelper.RemoveCookie(Invite.CookieName);
                //        m_LanguageCookie.InjectCookie(systemUser.Culture);

                //        var url = systemUser.UniversityId.HasValue
                //            ? Url.Action("Index", "Dashboard")
                //            : Url.Action("Choose", "University");
                //        return JsonOk(url);

                //    }
                //    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(AccountValidation.AccountError.InvalidPassword));
                //    return JsonError(GetErrorFromModelState());
                //}
                //if (systemUser.FacebookId.HasValue)
                //{
                //    ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailFacebookAccountError);
                //    return JsonError(GetErrorFromModelState());
                //}
                //if (!string.IsNullOrEmpty(systemUser.GoogleId))
                //{
                //    ModelState.AddModelError(string.Empty, AccountControllerResources.RegisterEmailGoogleAccountError);
                //    return JsonError(GetErrorFromModelState());
                //}
            }
            catch (Exception ex)
            {
                TraceLog.WriteError($"LogOn model : {model} ", ex);
                ModelState.AddModelError(string.Empty, "Logon Error");
            }
            return JsonError(GetErrorFromModelState());
        }
    }
}