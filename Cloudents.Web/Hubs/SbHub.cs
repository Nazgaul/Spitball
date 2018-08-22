using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Hubs
{
    public class SbHub :Hub
    {
        public const string MethodName = "Message";
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync(MethodName, message);
        }

        public async Task Message(object entity)
        {



            //var newObj = new
            //{
            //    type="entity",

            //}
            //await Clients.All.SendAsync(MethodName, new SignalRTransportType<object>("entity", SignalRAction.Add, dto))
        }
    }
}
