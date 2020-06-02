using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Chat
{
    public class ChatConversationDetailQuery : IQuery<ChatConversationDetailsDto?>
    {
        public ChatConversationDetailQuery(string id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private string Id { get; }
        private long UserId { get; }

        internal sealed class ChatConversationDetailQueryHandler : IQueryHandler<ChatConversationDetailQuery, ChatConversationDetailsDto?>
        {
            private readonly IStatelessSession _statelessSession;

            public ChatConversationDetailQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }

            public async Task<ChatConversationDetailsDto?> GetAsync(ChatConversationDetailQuery query, CancellationToken token)
            {
                var permissionFeature = _statelessSession.Query<ChatUser>()

                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.ChatRoom.Identifier == query.Id)
                    .ToFutureValue(f => f.Any());

                //var calendarFuture = _statelessSession.Query<GoogleTokens>()
                //    .Where(w=>w.Id == query.Id)

               var detailsFuture = _statelessSession.Query<ChatRoom>()
                    .Fetch(f => f.Tutor).ThenFetch(f => f.User)
                    .Where(w => w.Identifier == query.Id)
                    .Select(s => new ChatConversationDetailsDto
                    {
                        Email = s.Tutor.User.Email,
                        Id = s.Tutor.Id,
                        Name = s.Tutor.User.Name,
                        Image = s.Tutor.User.ImageName,
                        PhoneNumber = s.Tutor.User.PhoneNumber,
                        Calendar = _statelessSession.Query<GoogleTokens>().Any(w2 => w2.Id == s.Tutor.Id.ToString())
                    }).ToFutureValue();
               var studyRoomFuture = _statelessSession.Query<StudyRoom>()
                   .Where(w => w.Identifier == query.Id)
                   .Select(s => s.Id)
                   .ToFutureValue();


               var details = await detailsFuture.GetValueAsync(token);

               if (!permissionFeature.Value)
               {
                   return null;
               }
               details.StudyRoomId = studyRoomFuture.Value == Guid.Empty ? new Guid?() : studyRoomFuture.Value;
               return details;
            }
        }
    }
}