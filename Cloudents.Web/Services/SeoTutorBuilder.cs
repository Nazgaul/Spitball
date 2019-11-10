using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Web.Services
{

    public class SeoTutorBuilder : IBuildSeo
    {
        private readonly IStatelessSession _session;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeoTutorBuilder(IStatelessSession session, LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<string> GetUrls(int index)
        {
            var t = _session.Query<Tutor>()
                .Fetch(f => f.User)
                .Where(w => (!w.User.LockoutEnd.HasValue || DateTime.UtcNow >= w.User.LockoutEnd.Value))
                .Where(w => w.State == ItemState.Ok)
                .Take(SiteMapController.PageSize)
                .Skip(SiteMapController.PageSize * index)
                .Select(s => new { s.Id, s.User.Name });

            foreach (var item in t)
            {
                yield return _linkGenerator.GetUriByRouteValues(_httpContextAccessor.HttpContext, SeoTypeString.Tutor, new
                {
                    item.Id,
                    item.Name
                });

            }
        }
    }
}