using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Zbang.Zbox.Domain
{
    public class ChatRoom
    {
        public ChatRoom(Guid id, IEnumerable<ChatUser> users)
        {
            Id = id;
            Users = users;
        }
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "lastUpdate")]
        public DateTime LastUpdate { get; set; }
        [JsonProperty(PropertyName = "users")]
        public IEnumerable<ChatUser> Users { get; private set; }

    }

    public class ChatUser
    {
        public ChatUser(long id)
        {
            Id = id;
        }
        [JsonProperty(PropertyName = "id")]
        public long Id { get;private set; }
        [JsonProperty(PropertyName = "unreadCount")]
        public int UnreadCount { get; set; }
    }

    public class ChatMessage
    {
        public ChatMessage(Guid chatRoomId, long userId, string message)
        {
            ChatRoomId = chatRoomId;
            UserId = userId;
            Message = message;
            Time = DateTime.UtcNow;
        }

        [JsonProperty(PropertyName = "chatRoomId")]
        public Guid ChatRoomId { get; private set; }
        [JsonProperty(PropertyName = "userId")]
        public long UserId { get; private set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }
        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
    }

}
