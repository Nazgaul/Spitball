using System;
using System.Threading.Tasks;
using Cloudents.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class StudyRoomHub : Hub
    {
        private readonly IHttpContextAccessor _httpContext;

        public StudyRoomHub(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public override async Task OnConnectedAsync()
        {
            var request = _httpContext.HttpContext.Request;
            var cookieVal = request.Cookies[StudyRoomController.CookieName];
            if (cookieVal == null)
            {
                throw new ArgumentException();
            }
            
            await Groups.AddToGroupAsync(Context.ConnectionId, "xxx");
            var roomId = Guid.Parse(cookieVal);

            await Groups.AddToGroupAsync(this.Context.ConnectionId, cookieVal);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _httpContext.HttpContext.Response.Cookies.Delete(StudyRoomController.CookieName);
            var request = _httpContext.HttpContext.Request;
            var cookieVal = request.Cookies[StudyRoomController.CookieName];

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, cookieVal);
            await base.OnDisconnectedAsync(exception);
        }
    }
}