using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminConversationsQuery : IQueryAdmin<IEnumerable<ConversationDto>>
    {
        private int Page { get; }
        public string Country { get; }
        private ChatRoomStatus Status { get; }
        private string AssignTo { get; }
        private WaitingFor? ConversationStatus { get; }


        public AdminConversationsQuery(long userId, int page, string country, ChatRoomStatus status, string assignTo, WaitingFor? conversationStatus)
        {
            UserId = userId;
            Page = page;
            Country = country;
            Status = status;
            AssignTo = assignTo;
            ConversationStatus = conversationStatus;
        }
        private long UserId { get; }
        internal sealed class AdminAllConversationsQueryHandler : IQueryHandler<AdminConversationsQuery, IEnumerable<ConversationDto>>
        {
            private readonly IStatelessSession _statelessSession;


            public AdminAllConversationsQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }

            public async Task<IEnumerable<ConversationDto>> GetAsync(AdminConversationsQuery query, CancellationToken token)
            {

                var p = _statelessSession.Query<ViewConversation>();


                if (!string.IsNullOrEmpty(query.AssignTo))
                {
                    p = p.Where(w => w.AssignTo == query.AssignTo);
                }
                if (!string.IsNullOrEmpty(query.Country))
                {
                    p = p.Where(w => w.Country == query.Country);
                }

                //switch (query.AssignTo)
                //{
                //    case ChatRoomAssign.Unassigned:
                //        p = p.Where(w => w.AssignTo == null);
                //        break;
                //    case ChatRoomAssign.All:
                //        break;
                //    default:
                //        p = p.Where(w => w.AssignTo == query.AssignTo);
                //        break;
                //}

                //p = p.Where(w => w.Status == ChatRoomStatus2.SessionScheduled);

                if (query.Status != null)
                {
                    if (query.Status == ChatRoomStatus.New)
                    {
                        p = p.Where(w => w.Status == query.Status || w.Status == null);
                    }
                    else
                    {
                        p = p.Where(w => w.Status == query.Status);
                    }
                }
                else
                {
                    if (query.UserId > 0)
                    {

                    }
                    else
                    {
                        var v = ChatRoomStatus.GetActiveStatus().ToArray();
                        p = p.Where(w => v.Contains(w.Status) || w.Status == null);
                    }
                }


                switch (query.ConversationStatus)
                {
                    case WaitingFor.Tutor:
                        p = p.Where(w => w.ConversationStatus == 1);
                        break;
                    case WaitingFor.Student:
                        p = p.Where(w => w.ConversationStatus == 2);
                        break;
                    case WaitingFor.Conv:
                        p = p.Where(w => w.ConversationStatus > 2);
                        break;

                }

                if (query.UserId > 0)
                {
                    p = p.Where(w => w.TutorId == query.UserId || w.UserId == query.UserId);
                }
                return await p.Select(s => new ConversationDto()
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Status = s.Status,
                    LastMessage = s.LastMessage,
                    TutorId = s.TutorId,
                    AssignTo = s.AssignTo,
                    HoursFromLastMessage = s.HoursFromLastMessage,
                    RequestFor = s.RequestFor,
                    StudyRoomExists = s.StudyRoomExists,
                    TutorEmail = s.TutorEmail,
                    TutorName = s.TutorName,
                    TutorPhoneNumber = s.TutorPhoneNumber,
                    UserEmail = s.UserEmail,
                    UserName = s.UserName,
                    UserPhoneNumber = s.UserPhoneNumber,
                    ConversationStatus = s.ConversationStatus


                }).OrderByDescending(o => o.LastMessage)
                    .Take(20).Skip(20 * query.Page).ToListAsync(token);

                //                const string sql = @"
                //with cte as (
                //Select 
                //cr.Id , 
                //cra.status,
                //cra.AssignTo,
                //cr.Identifier ,
                //cr.UpdateTime as lastMessage,
                //u.id as userId,
                //u.Name,
                //u.Email,
                //u.PhoneNumberHash,
                //case when (select top 1 UserId from sb.ChatMessage cm where  cm.ChatRoomId = cr.id ) = cu.userid then 0 else 1 end as isTutor

                //from sb.ChatUser cu
                //join sb.ChatRoom cr on cu.ChatRoomId = cr.Id
                //left join sb.ChatRoomAdmin cra
                //	on cr.Id = cra.Id
                //join sb.[user] u on cu.UserId = u.Id
                //)
                //select c.Identifier as id,
                //c.lastMessage as lastMessage,
                //c.Name as UserName,
                //c.PhoneNumberHash as UserPhoneNumber,
                //c.Email as UserEmail,
                //c.userId as UserId,
                //d.Name as TutorName,
                //d.PhoneNumberHash as TutorPhoneNumber,
                //d.Email as TutorEmail,
                //d.UserId as TutorId,
                //c.status,
                //c.AssignTo,
                //(SELECT max (grp) FROM 
                //(
                //SELECT *, COUNT(isstart) OVER( PARTITION BY ChatRoomId ORDER BY Id ROWS UNBOUNDED PRECEDING) AS grp
                //FROM (
                //SELECT *,
                //CASE WHEN ABS(UserId - LAG(UserId) OVER(PARTITION BY ChatRoomId ORDER BY Id)) <= 1 THEN NULL ELSE 1 END AS isstart
                //FROM sb.ChatMessage
                //where ChatRoomId = c.id
                //) t1
                //) t2) as conversationStatus,
                //case when (Select  id from sb.StudyRoom where Identifier = c.Identifier) is null then 0 else 1 end  as studyRoomExists,
                //datediff(HOUR, c.lastMessage, GETUTCDATE()) as HoursFromLastMessage
                //from cte c 
                //inner join cte d on d.id = c.id and c.isTutor = 0 and d.isTutor = 1
                // where (c.UserId = @UserId or @UserId = 0 or d.userId = @UserId) 
                // and (c.AssignTo = @AssignTo or @AssignTo = 'all' or (@AssignTo = 'Unassigned' and (c.AssignTo is null or c.AssignTo = 'Unassigned'))) 
                //		and (c.status = @Status or @Status = 'all' or (@Status = 'Unassigned' and (c.status is null or c.status = 'Unassigned')))
                //		and ((SELECT max (grp) FROM 
                //(
                //SELECT *, COUNT(isstart) OVER( PARTITION BY ChatRoomId ORDER BY Id ROWS UNBOUNDED PRECEDING) AS grp
                //FROM (
                //SELECT *,
                //CASE WHEN ABS(UserId - LAG(UserId) OVER(PARTITION BY ChatRoomId ORDER BY Id)) <= 1 THEN NULL ELSE 1 END AS isstart
                //FROM sb.ChatMessage
                //where ChatRoomId = c.id
                //) t1
                //) t2) = @Conv or @Conv = 0 
                //or (@Conv = 3 and (SELECT max (grp) FROM 
                //(
                //SELECT *, COUNT(isstart) OVER( PARTITION BY ChatRoomId ORDER BY Id ROWS UNBOUNDED PRECEDING) AS grp
                //FROM (
                //SELECT *,
                //CASE WHEN ABS(UserId - LAG(UserId) OVER(PARTITION BY ChatRoomId ORDER BY Id)) <= 1 THEN NULL ELSE 1 END AS isstart
                //FROM sb.ChatMessage
                //where ChatRoomId = c.id
                //) t1
                //) t2) > 2))
                //order by c.lastMessage desc
                //OFFSET @pageSize * @PageNumber ROWS
                //FETCH NEXT @pageSize ROWS ONLY;";
                //                using (var connection = _dapper.OpenConnection())
                //                {
                //                    var res = await connection.QueryAsync<ConversationDto>(sql,
                //                        new
                //                        {
                //                            query.UserId,
                //                            pageSize = 50,
                //                            PageNumber = query.Page,
                //                            Status = query.Status.ToString(),
                //                            AssignTo = query.AssignTo.ToString(),
                //                            Conv = (int)query.ConversationStatus
                //                        });
                //                    return res;
                //                }
            }
        }
    }
}
