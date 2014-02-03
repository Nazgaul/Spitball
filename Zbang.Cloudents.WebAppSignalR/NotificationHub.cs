using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Zbang.Cloudents.WebAppSignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private const string Box = "Box";
        // private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();
      // private readonly Lazy<IShortCodesCache> m_ShortToLongCache;

       //public NotificationHub(Lazy<IShortCodesCache> shortToLongCache)
       //{
       // //   m_ShortToLongCache = shortToLongCache;
       //}

        public void FileUploaded(string boxid, object file)
        {

            Clients.OthersInGroup(Box + boxid).fileAdded(file,boxid);
        }

        public void RemoveFile(string boxid, string itemid)
        {
            Clients.OthersInGroup(Box + boxid).fileRemoved(itemid);
        }

        public void AddQuestion(string boxid, object question)
        {
            Clients.OthersInGroup(Box + boxid).questionAdded(boxid, question);
        }

        public void RemoveQuestion(string boxid, Guid question)
        {
            Clients.OthersInGroup(Box + boxid).questionRemoved(question);
        }

        public void AddAnswer(string boxid, Guid questionId, object answer)
        {
            Clients.OthersInGroup(Box + boxid).answerAdded(questionId, answer, boxid);
        }

        public void RemoveAnswer(string boxid, Guid questionId, Guid answerId)
        {
            Clients.OthersInGroup(Box + boxid).answerRemoved(questionId, answerId);
        }

        public void InviteUser(long userUid)
        {
            //var userId = m_ShortToLongCache.Value.ShortCodeToLong(userUid, ShortCodesType.User);
            Clients.User(userUid.ToString()).inviteUser();
        }


        public async Task JoinBox(IList<string> boxids)
        {
           var tasks = new List<Task>();
           foreach (var boxid in boxids)
           {
               tasks.Add(Groups.Add(Context.ConnectionId, Box + boxid));
           }
           await Task.WhenAll(tasks);
        }

        public override Task OnConnected()
        {

            //string userName = Context.User.Identity.Name;
            //string connectionId = Context.ConnectionId;

            //var user = Users.GetOrAdd(userName, _ => new User
            //{
            //    Name = userName,
            //    ConnectionIds = new HashSet<string>()
            //});

            //lock (user.ConnectionIds)
            //{

            //    user.ConnectionIds.Add(connectionId);

            //    if (user.ConnectionIds.Count == 1)
            //    {
            //        // Clients.Others.userConnected(userName);
            //    }
            //}

            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            //string userName = Context.User.Identity.Name;
            //string connectionId = Context.ConnectionId;

            //User user;
            //Users.TryGetValue(userName, out user);

            //if (user != null)
            //{

            //    lock (user.ConnectionIds)
            //    {

            //        user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

            //        if (!user.ConnectionIds.Any())
            //        {

            //            User removedUser;
            //            Users.TryRemove(userName, out removedUser);

            //            // You might want to only broadcast this info if this 
            //            // is the last connection of the user and the user actual is 
            //            // now disconnected from all connections.
            //            //Clients.Others.userDisconnected(userName);
            //        }
            //    }
            //}

            return base.OnDisconnected();
        }


    }



}