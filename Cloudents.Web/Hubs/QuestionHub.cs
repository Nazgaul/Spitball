using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cloudents.Web.Hubs
{
    public class QuestionHub :Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
