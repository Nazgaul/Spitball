﻿using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class ChatRoomRepository : NHibernateRepository<ChatRoom>, IChatRoomRepository
    {
        public ChatRoomRepository(ISession session) : base(session)
        {
        }


        //public Task<ChatRoom?> GetChatRoomAsync(IEnumerable<long> usersId, CancellationToken token)
        //{
        //    var identifier = ChatRoom.BuildChatRoomIdentifier(usersId);
        //    return GetChatRoomAsync(identifier, token);
        //}

        public async Task<ChatRoom> GetOrAddChatRoomAsync(IEnumerable<long> userIds, Tutor tutor, CancellationToken token)
        {
            var userIdsLists = userIds.ToList();
            var identifier = ChatRoom.BuildChatRoomIdentifier(userIdsLists);
            var chatRoom = await GetChatRoomAsync(identifier, token);
            if (chatRoom == null)
            {
                var users = userIdsLists.Select(s => Session.Load<User>(s)).ToList();
                chatRoom = new ChatRoom(users, tutor);
                await AddAsync(chatRoom, token);
            }

            return chatRoom;
        }

        public Task<ChatRoom> GetOrAddChatRoomAsync(IEnumerable<long> userIds, CancellationToken token)
        {
            var userIdsLists = userIds.ToList();
            var tutor = userIdsLists.Select(s => Session.Get<Tutor>(s)).Single(w => w != null);
            return GetOrAddChatRoomAsync(userIdsLists, tutor, token);
            //var identifier = ChatRoom.BuildChatRoomIdentifier(userIdsLists);
            //var chatRoom = await GetChatRoomAsync(identifier, token);

            //if (chatRoom == null)
            //{
            //    var users = userIdsLists.Select(s => Session.Load<User>(s)).ToList();

            //    chatRoom = new ChatRoom(users, );
            //    await AddAsync(chatRoom, token);
            //}

            //return chatRoom;
        }


        public Task<ChatRoom?> GetChatRoomAsync(string identifier, CancellationToken token)
        {
            return Session.Query<ChatRoom>()
                .Fetch(f => f.Extra)
                .Where(t => t.Identifier == identifier).SingleOrDefaultAsync(token);
        }

        public Task UpdateNonDayOldConversationToActiveAsync(CancellationToken token)
        {
            return Session.Query<ChatRoomAdmin>()
                .Where(w => (Session.Query<ChatRoom>()
                    .Where(w2 => w2.UpdateTime > DateTime.UtcNow.AddDays(-1)).Select(s => s.Id).Contains(w.Id)))
                .UpdateAsync(x => new { Status = ChatRoomStatus.New }, token);
        }


    }
}