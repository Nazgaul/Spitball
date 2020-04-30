using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SetSessionDurationCommandHandler : ICommandHandler<SetSessionDurationCommand>
    {
        private readonly IRepository<StudyRoomSession> _repository;
        public SetSessionDurationCommandHandler(IRepository<StudyRoomSession> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(SetSessionDurationCommand message, CancellationToken token)
        {
            var session = await _repository.LoadAsync(message.SessionId, token);
            if (session.StudyRoom.Tutor.Id != message.TutorId)
            {
                throw new ArgumentException();
            }

            if (session.StudyRoomVersion == StudyRoomSession.StudyRoomNewVersion)
            {
                var userSession = session.RoomSessionUsers.AsQueryable().Single(s => s.User.Id == message.UserId);
                userSession.ApproveSession(message.RealDuration , message.Price);
            }
            else
            {
                session.SetRealDuration(message.RealDuration, message.Price);
            }

            await _repository.UpdateAsync(session, token);
        }
    }
}
