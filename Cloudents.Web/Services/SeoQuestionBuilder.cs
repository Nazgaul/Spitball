using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Web.Services
{
    public class SeoQuestionBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoQuestionBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<string> GetUrls(int index)
        {
            var t = _session.Query<Question>()
                .Where(w => w.Status.State == ItemState.Ok)
                .Take(SiteMapController.PageSize).Skip(SiteMapController.PageSize * index)
                .Select(s => s.Id);

            foreach (var item in t)
            {
                yield return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Question, new
                {
                    Id = item,
                });

            }
        }
    }
}