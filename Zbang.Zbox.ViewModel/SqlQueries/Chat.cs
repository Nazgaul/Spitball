﻿namespace Zbang.Zbox.ViewModel.SqlQueries
{
    public static class Chat
    {
        public const string GetUsersConversationAndFriends =
            @"SELECT 1 as section, e2.chatroomid as Conversation,cr.UpdateTime,  e1.Unread, u.userid as id,username as name, UserImageLarge as image, online, Url as url ,LastAccessTime as lastSeen
FROM zbox.ChatUser e1, 
zbox.ChatUser e2 join zbox.Users u on e2.UserId = u.userid 
join zbox.ChatRoom cr on e2.ChatRoomId = cr.Id
WHERE e1.ChatRoomId = e2.ChatRoomId 
AND e1.id != e2.id
and e1.userid = @UserId
and u.UserName like  @term + '%'
union all
select distinct 2 as section, null as Conversation, null,  null as Unread ,u.userid as id,username as name, UserImageLarge as image, online, Url as url,LastAccessTime as lastSeen
from zbox.users u
join zbox.userboxrel ub1 on ub1.userid = u.userid
join zbox.userboxrel ub2 on ub1.boxid = ub2.boxid
and ub2.userid = @UserId
and u.UserName like  @term + '%'
union all
select 3 as section, null as Conversation, null,  null as Unread ,u.userid as id,username as name, UserImageLarge as image, online, Url as url,LastAccessTime as lastSeen
from zbox.users u
where u.UniversityId = @Universityid
and u.userid <> @UserId
and u.UserName like  @term + '%'
order by section, cr.UpdateTime desc , u.online desc, u.LastAccessTime desc
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY;";

        public const string GetUsersConversationAndFriendsWithoutTerm =
    @"SELECT 1 as section, e2.chatroomid as Conversation,cr.UpdateTime,  e1.Unread, u.userid as id,username as name, UserImageLarge as image, online, Url as url ,LastAccessTime as lastSeen
FROM zbox.ChatUser e1, 
zbox.ChatUser e2 join zbox.Users u on e2.UserId = u.userid 
join zbox.ChatRoom cr on e2.ChatRoomId = cr.Id
WHERE e1.ChatRoomId = e2.ChatRoomId 
AND e1.id != e2.id
and e1.userid = @UserId
union all
select distinct 2 as section, null as Conversation, null,  null as Unread ,u.userid as id,username as name, UserImageLarge as image, online, Url as url,LastAccessTime as lastSeen
from zbox.users u
join zbox.userboxrel ub1 on ub1.userid = u.userid
join zbox.userboxrel ub2 on ub1.boxid = ub2.boxid
and ub2.userid = @UserId
union all
select 3 as section, null as Conversation, null,  null as Unread ,u.userid as id,username as name, UserImageLarge as image, online, Url as url,LastAccessTime as lastSeen
from zbox.users u
where u.UniversityId = @Universityid
and u.userid <> @UserId
order by section, cr.UpdateTime desc , u.online desc, u.LastAccessTime desc
offset @PageNumber*@RowsPerPage ROWS
FETCH NEXT @RowsPerPage ROWS ONLY;";




        public const string GetUserFriends =
            @"select top 50 null as Conversation, null as Unread ,u.userid as id,username as name, UserImageLarge as image, online, Url as url
from zbox.users u
where u.UniversityId = @UniversityId
and u.userid <> @UserId
order by u.UserName";


        public const string GetChat = @"select Id, Message as text,CreationTime as Time ,UserId, blob as blob from zbox.ChatMessage
where ChatRoomId = @ChatRoom
and (@creationTime is null or CreationTime< @creationTime)
order by id desc
offset 0 ROWS
FETCH NEXT @top ROWS ONLY; ";

        public const string GetChatByUserIds =
            @"select Id, Message as text,CreationTime as Time ,UserId, blob as blob from zbox.ChatMessage
where ChatRoomId = ( 
select chatroomid from zbox.ChatUser where UserId in  @UserIds
group by chatroomid
having count(*) = @length)
and (@creationTime is null or CreationTime< @creationTime)
order by id desc
offset 0 ROWS
FETCH NEXT @top ROWS ONLY;";


        public const string GetUnreadMessages = "select sum(unread) from zbox.ChatUser where userid = @UserId";
    }
}
