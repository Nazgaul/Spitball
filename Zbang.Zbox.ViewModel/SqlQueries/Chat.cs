namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Chat
    {
        public const string GetUsersWithConversation = @"
SELECT e2.chatroomid as Conversation,e1.Unread, u.userid as id,username as name, UserImageLarge as image, online, Url as url FROM zbox.ChatUser e1, zbox.ChatUser e2 join zbox.Users u on e2.UserId = u.userid
WHERE e1.ChatRoomId = e2.ChatRoomId 
AND e1.id != e2.id
and e1.userid = @UserId";


        public const string GetChat = @"select Message as text,CreationTime as Time ,UserId from zbox.ChatMessage
where ChatRoomId = @ChatRoom";


        public const string GetUnreadMessages = "select sum(unread) from zbox.ChatUser where userid = @UserId";
    }
}
