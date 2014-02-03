using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole.RT
{
    [Authorize]
    
    public class ZboxHub : Hub
    {
        //private readonly IZboxReadService m_ZboxReadService;
        public ZboxHub(/*IZboxReadService zboxReadService*/)
        {
            //m_ZboxReadService = zboxReadService;
        }
        public void Hello()
        {
            Clients.All.hello();
        }

        public Task JoinBox(string id)
        {
            return Groups.Add(Context.ConnectionId, GroupConsts.Box + id);
        }

        public Task LeaveBox(string id)
        {
            return Groups.Remove(Context.ConnectionId, GroupConsts.Box + id);
        }

        public override Task OnConnected()
        {
            
            // need to add user to rooms
            //add call to show them to other users
           //var userId = long.Parse( Context.User.Identity.Name);
           //var userBoxes =  m_ZboxReadService.GetBoxes(new Zbox.ViewModel.Queries.Boxes.GetBoxesQuery(userId,0,Zbox.Infrastructure.Enums.OrderBy.LastModified);

            
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            //disconnect user from rooms
            return base.OnDisconnected();
        }
    }
}