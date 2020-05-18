using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Query;
using Cloudents.Query.Users;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Cloudents.Web.Filters
{
    public class GenerateFeedResultFilter : IAsyncActionFilter
    {
        private readonly IUrlBuilder _urlBuilder;
        private readonly LinkGenerator _linkGenerator;
        private readonly IQueryBus _queryBus;
        private readonly UserManager<User> _userManager;

        public GenerateFeedResultFilter(IUrlBuilder urlBuilder, LinkGenerator linkGenerator, IQueryBus queryBus, UserManager<User> userManager)
        {
            _urlBuilder = urlBuilder;
            _linkGenerator = linkGenerator;
            _queryBus = queryBus;
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //throw new System.NotImplementedException();
            //}
            //public override void OnActionExecuted(ActionExecutedContext context)
            //{
            var resultContext = await next();
           
            var result = resultContext.Result;
            if (result is ObjectResult objResult)
            {
                if (objResult.Value is WebResponseWithFacet<FeedDto> feed)
                {
                    _userManager.TryGetLongUserId(context.HttpContext.User, out var userId);
                    var query = new UserSubscribersQuery(userId);
                    var subscriberResult = await _queryBus.QueryAsync(query, context.HttpContext.RequestAborted);
                    feed.Result = feed.Result.Select(s =>
                    {
                        if (s is DocumentFeedDto p)
                        {
                            if (subscriberResult.Contains(p.User.Id))
                            {
                                p.PriceType = PriceType.Free;
                            }
                            p.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(p.Id);
                            p.Url = _linkGenerator.GetUriByRouteValues(context.HttpContext, SeoTypeString.Document, new
                            {
                                courseName = FriendlyUrlHelper.GetFriendlyTitle(p.Course),
                                id = p.Id,
                                name = FriendlyUrlHelper.GetFriendlyTitle(p.Title)
                            });

                        }

                        return s;
                    });
                    // p.Url = Url.DocumentUrl(p.Course, p.Id, p.Title);
                }

            }

           
            // ((ObjectResult)result).Value
            //base.OnActionExecuted(context);
        }


    }
}