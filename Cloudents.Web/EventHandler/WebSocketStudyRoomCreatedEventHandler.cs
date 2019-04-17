using Cloudents.Core;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query;

namespace Cloudents.Web.EventHandler
{

    public class WebSocketStudyRoomCreatedEventHandler : IEventHandler<StudyRoomCreatedEvent>
    {
        private readonly IHubContext<SbHub> _hubContext;
        private readonly IChatRoomRepository _chatRoomRepository;// This is ok for now because we are still inside session

        public WebSocketStudyRoomCreatedEventHandler(IHubContext<SbHub> hubContext, IChatRoomRepository chatRoomRepository)
        {
            _hubContext = hubContext;
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task HandleAsync(StudyRoomCreatedEvent eventMessage, CancellationToken token)
        {
            
            var studyRoom = eventMessage.StudyRoom;
            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(studyRoom.Identifier, token);
            var message = new SignalRTransportType(SignalRType.StudyRoom,
                SignalRAction.Add, new
                {
                    id = studyRoom.Id,
                    userId = studyRoom.Tutor.Id,
                    conversationId = chatRoom.Id
                });
            await _hubContext.Clients.Users(studyRoom.Users.Select(s => s.User.Id.ToString()).ToList()).SendAsync(SbHub.MethodName, message, token);
        }
    }
}