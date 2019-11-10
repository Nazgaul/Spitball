using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ViewConversationMap : ClassMap<ViewConversation>
    {
        public ViewConversationMap()
        {
            Id(x => x.Id);
            Map(x => x.LastMessage);
            Map(x => x.UserName);
            Map(x => x.UserPhoneNumber);
            Map(x => x.UserEmail);
            Map(x => x.UserId);
            Map(x => x.TutorName);
            Map(x => x.TutorPhoneNumber);
            Map(x => x.TutorEmail);
            Map(x => x.TutorId);
            Map(x => x.Status).CustomType<EnumerationType<ChatRoomStatus>>();
            Map(x => x.AssignTo);
            Map(x => x.RequestFor);
            Map(x => x.ConversationStatus);
            Map(x => x.StudyRoomExists);
            Map(x => x.HoursFromLastMessage);
            Map(x => x.Country);
            SchemaAction.Validate();
            Table("vAdminConversation");
            ReadOnly();

            /*
           alter VIEW [sb].[vAdminConversation] 

as
with cte as (
Select 
cr.Id , 
cra.status2 as status,
cra.AssignTo,
cr.Identifier ,
cr.UpdateTime as lastMessage,
u.id as userId,
u.Name,
u.Email,
u.PhoneNumberHash,
CONCAT(l.CourseId, ' ', l.Text) as RequestFor,
case when (select top 1 UserId from sb.ChatMessage cm where  cm.ChatRoomId = cr.id ) = cu.userid then 0 else 1 end as isTutor

from sb.ChatUser cu
join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
left join sb.ChatRoomAdmin cra
	on cr.Id = cra.Id
join sb.[user] u on cu.UserId = u.Id
left join sb.Lead l on cra.LeadId = l.Id
)
select c.Identifier as id,
c.lastMessage as lastMessage,
c.Name as UserName,
c.PhoneNumberHash as UserPhoneNumber,
c.Email as UserEmail,
c.userId as UserId,
d.Name as TutorName,
d.PhoneNumberHash as TutorPhoneNumber,
d.Email as TutorEmail,
d.UserId as TutorId,
c.status,
c.AssignTo,
c.RequestFor,
(SELECT max (grp) FROM 
(
SELECT *, COUNT(isstart) OVER( PARTITION BY ChatRoomId ORDER BY Id ROWS UNBOUNDED PRECEDING) AS grp
FROM (
SELECT *,
CASE WHEN ABS(UserId - LAG(UserId) OVER(PARTITION BY ChatRoomId ORDER BY Id)) <= 1 THEN NULL ELSE 1 END AS isstart
FROM sb.ChatMessage
where ChatRoomId = c.id
) t1
) t2) as conversationStatus,
case when (Select  id from sb.StudyRoom where Identifier = c.Identifier) is null then 0 else 1 end  as studyRoomExists,
datediff(HOUR, c.lastMessage, GETUTCDATE()) as HoursFromLastMessage
from cte c 
inner join cte d on d.id = c.id and c.isTutor = 0 and d.isTutor = 1
             */
        }
    }
}